using System;
using System.Collections;
using UnityEngine;

public static class AudioClipExpand
{
    public static IEnumerator GetSamples(this AudioClip _clip,int _samplesCount,Action<float[]> _callback)
    {
        float[] samples = new float[_samplesCount];
        float[] allSamples = new float[_clip.samples*_clip.channels];
        _clip.GetData(allSamples,0);
        int space = allSamples.Length/samples.Length;
        double sum=0;
        long count=0;
        var waitFrame = new WaitForEndOfFrame();
        for (int i = 0; i < samples.Length; i++)
        {
            for(int j=0;j<space;j++)
            {
                sum+=Mathf.Abs(allSamples[i*space+j]);
            }
            samples[i]=(float)(sum/space);
            sum=0;
            count++;
            if(count%(6*10)==0)
            {
                yield return waitFrame;
            }
        }
        _callback(samples);
    }
}