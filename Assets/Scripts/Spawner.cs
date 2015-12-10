﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    public List<Particle> particles = new List<Particle>();
    public List<Spring> springs = new List<Spring>();
    public Particle particle;
    public Spring spring;
    public int width, height;
    public Slider k, b, l, mass;

    void Awake()
    {
        Spawn();
        particles[0].isPinned = true;
        particles[4].isPinned = true;
        particles[20].isPinned = true;
        particles[24].isPinned = true;
    }

    void FixedUpdate()
    {
        foreach (Particle p in particles)
        {
            p.mass = mass.value;
            p.force = new Vector3(0, -9.8f, 0) * p.mass;
        }

        foreach (Spring s in springs)
        {
            s.GetComponent<Spring>().ComputeForce();
            s.springConstant = k.value;
            s.dampingFactor = b.value;
            s.RestLength = l.value;
        }

        foreach (Particle p in particles) // Particle Euler Intergration
        {
            p.GetComponent<Particle>().ParticleMath();
        }
    }

    // Loops through spring list to draw lines to each particle
    void Update()
    {
        int i = 0;
        foreach (Spring s in springs)
        {
            LineRenderer l = s.GetComponent<LineRenderer>();
            l.SetPosition(0, springs[i].p1.transform.position);
            l.SetPosition(1, springs[i].p2.transform.position);
            i++;
        }

        if (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.P))
        {
            Application.Quit();
        }
    }

    public void Spawn()
    {
        GameObject particles_ = new GameObject();
        particles_.name = "Particles";
        particles_.transform.SetParent(transform);

        GameObject springs_ = new GameObject();
        springs_.name = "Springs";
        springs_.transform.SetParent(transform);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Particle newParticle = Instantiate(particle);
                newParticle.transform.position = new Vector3(x * width, y * height, 0);
                newParticle.name = (x * width).ToString() + " " + (y * height).ToString(); // Set all particle clone names into numbers
                newParticle.transform.SetParent(particles_.transform);
                particles.Add(newParticle);

                /*
                
                6 <-- 7 <-- 8
                | \   |   / |
                3 <-- 4 <-- 5
                |  \  |   / |
                0 <-- 1 <-- 2

                Left Springs = Springs are created and link to the previous one made
                1 to 0
                2 to 1

                Above Springs = Springs are connected to the bottom one
                3 to 0
                4 to 1
                5 to 2

                DownLeftDiagonal = Springs connected from top to bottom left
                4 to 0
                5 to 1
                7 to 3
                8 to 4

                DownRightDiagonal = Springs connected from top to bottom right
                3 to 1
                4 to 2
                6 to 4
                7 to 5
                */

                if (x > 0) // Left spring
                {
                    // Make sure this isn't the first particle in the row, if there is, there won't be anything to it's left and we can't connect.
                    //connects to the part we just created with the particle at the index.
                    // particle at the same Y value.
                    // x - 1 we're creating all the particles one by one, but storing.
                    Spring sideSpring = Instantiate(spring);
                    LineRenderer l = sideSpring.GetComponent<LineRenderer>();
                    l.SetPosition(0, newParticle.transform.position);
                    l.SetPosition(1, particles[y * width + (x - 1)].transform.position);
                    sideSpring.name = "SideSpring: " + x.ToString() + " " + y.ToString();
                    sideSpring.makeSpring(newParticle, particles[y * width + (x - 1)]);
                    springs.Add(sideSpring);
                    sideSpring.transform.SetParent(springs_.transform);
                }

                if (y > 0) // Up Spring
                {
                    //// Connecting ontop, but going downwards.
                    Spring aboveSpring = Instantiate(spring);
                    LineRenderer l = aboveSpring.GetComponent<LineRenderer>();
                    l.SetPosition(0, newParticle.transform.position);
                    l.SetPosition(1, particles[(y - 1) * width + x].transform.position);
                    aboveSpring.name = "AboveSpring: " + x.ToString() + " " + y.ToString();
                    aboveSpring.makeSpring(newParticle, particles[(y - 1) * width + x]);
                    springs.Add(aboveSpring);
                    aboveSpring.transform.SetParent(springs_.transform);
                }

                if (x > 0 && y > 0) // DownLeftDiagonal spring y-1, x-1
                {
                    Spring downLeftSpring = Instantiate(spring);
                    LineRenderer l = downLeftSpring.GetComponent<LineRenderer>();
                    l.SetPosition(0, newParticle.transform.position);
                    l.SetPosition(1, particles[(y - 1) * width + (x - 1)].transform.position);
                    downLeftSpring.name = "UpLeftSpring: " + x.ToString() + " " + y.ToString();
                    downLeftSpring.makeSpring(newParticle, particles[(y - 1) * width + (x - 1)]);
                    springs.Add(downLeftSpring);
                    downLeftSpring.transform.SetParent(springs_.transform);
                }

                if (x < width - 1 && y > 0) // DownRightDiagonal, spring y-1, x+1
                {
                    Spring downRightSpring = Instantiate(spring);
                    LineRenderer l = downRightSpring.GetComponent<LineRenderer>();
                    l.SetPosition(0, newParticle.transform.position);
                    l.SetPosition(1, particles[(y - 1) * width + (x + 1)].transform.position);
                    downRightSpring.name = "DownRightSpring: " + x.ToString() + " " + y.ToString();
                    downRightSpring.makeSpring(newParticle, particles[(y - 1) * width + (x + 1)]);
                    springs.Add(downRightSpring);
                    downRightSpring.transform.SetParent(springs_.transform);
                }
            }
        }
    }
}