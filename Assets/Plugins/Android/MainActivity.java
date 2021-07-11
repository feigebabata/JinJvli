package com.fgbbt.JinJvLi; //包名 改称自己的

import android.content.Context;
import android.net.wifi.WifiManager;
import android.os.Bundle;
import android.util.Log;

import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;

public class MainActivity extends UnityPlayerActivity
{
    WifiManager.MulticastLock lock;
    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        Log.d("Unity","安卓 活动 初始化");
        super.onCreate(savedInstanceState);

        WifiManager manager = (WifiManager) this.getApplicationContext().getSystemService(Context.WIFI_SERVICE);
        lock= manager.createMulticastLock("test wifi");

        UnityPlayer.UnitySendMessage("AndroidBehaviour","OnAndroidMsg","安卓消息");
    }

    @Override
    protected void onDestroy()
    {
        super.onDestroy();
        lock.release();
    }

    public void lockAcquire()
    {
        lock.acquire();
    }

    public void lockRelease()
    {
        lock.release();
    }
}
