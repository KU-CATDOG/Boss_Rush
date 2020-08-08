﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBoss : Boss
{
    [SerializeField]
    private GameObject normalBullet;
    [SerializeField]
    private Vector3 shootPos;
    [SerializeField]
    private GameObject laserCollider;
    [SerializeField]
    private Bounds map;
    [SerializeField]
    private LineRenderer lr, lr2;
    const float laserReadyWidth = 0.1f;
    const float laserShootWidth = 2.0f;

    const float bulletSpeed = 5;





    protected override Queue<IEnumerator> DecideNextRoutine()
    {
        Queue<IEnumerator> nextRoutines = new Queue<IEnumerator>();

        float rand = Random.value;

        switch (1)
        {
            case 0:

                if (rand < 0.33f)
                {

                    nextRoutines.Enqueue(NewActionRoutine(CircularShotRoutine(5, 12, 1f)));
                    nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));

                }
                else if (rand < 0.66f)
                {
                    nextRoutines.Enqueue(NewActionRoutine(StraightShotRoutine(6, 0.1f)));
                    nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));

                }
                else
                {
                    nextRoutines.Enqueue(NewActionRoutine(FanShotRoutine(5, 5, 0.7f)));
                    nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                }

                nextRoutines.Enqueue(NewActionRoutine(IdleRoutine(0.8f)));
                break;

            case 1:

                Vector3 distance = shootPos - GetPlayerPos();

                if(rand < 0.5f)
                {
                    nextRoutines.Enqueue(NewActionRoutine(LaserRoutine(1.0f)));
                    nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                }
                else
                {
                    if (distance.magnitude > 9)
                    {
                        nextRoutines.Enqueue(NewActionRoutine(LaserRoutine2(1.0f)));
                        nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                    }
                    else
                    {
                        nextRoutines.Enqueue(NewActionRoutine(LaserRoutine2b(1.0f)));
                        nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1.0f)));
                    }
                }
                
                
                break;
        }

        return nextRoutines;
    }


    private IEnumerator CircularShotRoutine(int numWave, int bulletCount, float interval)
    {
        for(int i=0; i<numWave; i++)
        {
            for (int j = 0; j < bulletCount; j++)
            {
                Instantiate(normalBullet, shootPos + new Vector3(Mathf.Cos(j * 30 * Mathf.PI / 180), Mathf.Sin(j * 30 * Mathf.PI / 180), 0), Quaternion.identity).GetComponent<Rigidbody2D>().velocity
                    = (new Vector3(Mathf.Cos(j * 30 * Mathf.PI / 180), Mathf.Sin(j * 30 * Mathf.PI / 180), 0)).normalized * bulletSpeed;
                //yield return null;
            }
            yield return new WaitForSeconds(interval);

        }

    }
    private IEnumerator StraightShotRoutine(int bulletCount, float interval)
    {

        for(int i=0; i < bulletCount; i++)
        {
            Instantiate(normalBullet, shootPos, Quaternion.identity).GetComponent<Rigidbody2D>().velocity = (GetPlayerPos() - shootPos).normalized * bulletSpeed * 2;
            yield return new WaitForSeconds(interval);
        }

    }

    private IEnumerator FanShotRoutine(int numWave, int bulletCount, float interval)
    {
        for(int i=0; i<numWave; i++)
        {
            for (int j = 0; j < bulletCount; j++)
            {
                Instantiate(normalBullet, shootPos, Quaternion.identity).GetComponent<Rigidbody2D>().velocity 
                    = (GetPlayerPos() + new Vector3(0, j-2, 0) / 0.5f * Mathf.Pow(-1, i) - shootPos).normalized * bulletSpeed * 1.5f;
                yield return new WaitForSeconds(0.15f);
            }
        yield return new WaitForSeconds(interval);
        }

    }

    private IEnumerator LaserRoutine(float waitTime)
    {
        lr.enabled = true;

        Vector3 lineEndPos = GetPlayerPos();

        lr.SetPosition(0, shootPos);
        lr.SetPosition(1, lineEndPos);

        for (float t = 0; t < waitTime; t += Time.deltaTime)
        {

            lineEndPos = GetPlayerPos();
            lr.SetPosition(0, shootPos);
            lr.SetPosition(1, lineEndPos);
            yield return null;
        }


        Vector3 diff = lineEndPos - shootPos;


        lr.startWidth = laserShootWidth;
        lr.endWidth = laserShootWidth;

        yield return new WaitForSeconds(0.5f);

        for (float t = 0; t < 0.2f; t += Time.deltaTime)
        {
            float size = Mathf.Lerp(laserShootWidth, laserReadyWidth, t / 0.2f);
            lr.startWidth = size;
            lr.endWidth = size;
            yield return null;
        }

        lr.startWidth = laserReadyWidth;
        lr.endWidth = laserReadyWidth;
        lr.enabled = false;
    }
    
    private IEnumerator LaserRoutine2(float waitTime)
    {
        lr.enabled = true;
        lr2.enabled = true;

        Vector3 end1 = new Vector3(map.min.x - 1, map.min.y, 0);
        Vector3 end2 = new Vector3(map.max.x + 1, map.min.y, 0);


        lr.SetPosition(0, shootPos);
        lr.SetPosition(1, end1);

        lr2.SetPosition(0, shootPos);
        lr2.SetPosition(1, end2);

        yield return new WaitForSeconds(waitTime);


        Vector3 fin = new Vector3(0, map.min.y, 0);

        lr.startWidth = laserShootWidth;
        lr.endWidth = laserShootWidth;
        
        lr2.startWidth = laserShootWidth;
        lr2.endWidth = laserShootWidth;

        while (end1.x < 0)
        {
            end1 = end1 + new Vector3(0.1f, 0, 0);
            end2 = end2 - new Vector3(0.1f, 0, 0);
            lr.SetPosition(1, end1);
            lr2.SetPosition(1, end2);

            yield return new WaitForSeconds(0.015f);
        }


        for (float t = 0; t < 0.2f; t += Time.deltaTime)
        {
            float size = Mathf.Lerp(laserShootWidth, laserReadyWidth, t / 0.2f);
            lr.startWidth = size;
            lr.endWidth = size;
            
            lr2.startWidth = size;
            lr2.endWidth = size;
            yield return null;
        }

        lr.startWidth = laserReadyWidth;
        lr.endWidth = laserReadyWidth;
        lr.enabled = false;
        
        lr2.startWidth = laserReadyWidth;
        lr2.endWidth = laserReadyWidth;
        lr2.enabled = false;
    }

    private IEnumerator LaserRoutine2b(float waitTime)
    {
        lr.enabled = true;
        lr2.enabled = true;

        Vector3 start = new Vector3(0, map.min.y, 0);

        lr.SetPosition(0, shootPos);
        lr.SetPosition(1, start);

        lr2.SetPosition(0, shootPos);
        lr2.SetPosition(1, start);

        yield return new WaitForSeconds(waitTime);

        Vector3 end1 = new Vector3(0, map.min.y, 0);
        Vector3 end2 = new Vector3(0, map.min.y, 0);

        lr.startWidth = laserShootWidth;
        lr.endWidth = laserShootWidth;

        lr2.startWidth = laserShootWidth;
        lr2.endWidth = laserShootWidth;

        while (end1.x > map.min.x - 1)
        {
            end1 = end1 - new Vector3(0.1f, 0, 0);
            end2 = end2 + new Vector3(0.1f, 0, 0);
            lr.SetPosition(1, end1);
            lr2.SetPosition(1, end2);

            yield return new WaitForSeconds(0.015f);
        }


        for (float t = 0; t < 0.2f; t += Time.deltaTime)
        {
            float size = Mathf.Lerp(laserShootWidth, laserReadyWidth, t / 0.2f);
            lr.startWidth = size;
            lr.endWidth = size;

            lr2.startWidth = size;
            lr2.endWidth = size;
            yield return null;
        }

        lr.startWidth = laserReadyWidth;
        lr.endWidth = laserReadyWidth;
        lr.enabled = false;

        lr2.startWidth = laserReadyWidth;
        lr2.endWidth = laserReadyWidth;
        lr2.enabled = false;
    }


    private IEnumerator IdleRoutine(float time)
    {
        yield return new WaitForSeconds(time);
    }

    protected override void OnStunned()
    {

    }

}



