using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdManager
{
    public void Init()
    {
        InitializeAds();
    }

    private bool _isInitialized;
    private readonly TimeSpan TIMEOUT = TimeSpan.FromHours(4);
    private DateTime _appOpenAdLoadTime;

    private AppOpenAd _appOpenAd;

    private const string APP_OPEN_AD_TEST_ID = "ca-app-pub-3940256099942544/9257395921";
    private const string INTERSTITIAL_AD_TEST_ID = "";
    private const string REWARDED_AD_TEST_ID = "";

    private const string APP_OPEN_AD_ID = ""; // 실제 앱 오프닝 광고 ID
    private const string INTERSTITIAL_AD_ID = ""; // 실제 전면 광고 ID
    private const string REWARDED_AD_ID = ""; // 실제 보상형 광고 ID

    public const bool TEST_MODE = true; // 테스트 모드 활성화 여부
    public const bool BLOCK_ADS = false; // 광고 차단 여부

    internal static List<string> TestDeviceIds = new List<string>()
    {
        AdRequest.TestDeviceSimulator,
        #if UNITY_IPHONE
        "96e23e80653bb28980d3f40beb58915c",
        #elif UNITY_ANDROID
        "702815ACFC14FF222DA1DC767672A573"
        #endif
    };

    private string GetAdUnitId(string testId, string liveId) => TEST_MODE ? testId : liveId;

    /// <summary>
    /// Google Mobile Ads 초기화
    /// </summary>
    private void InitializeAds()
    {
        if (_isInitialized) return;

        // 테스트 디바이스 설정
        MobileAds.SetRequestConfiguration(new RequestConfiguration
        {
            TestDeviceIds = TestDeviceIds
        });

        Debug.Log("Google Mobile Ads 초기화 중...");
        MobileAds.Initialize(status =>
        {
            Debug.Log("Google Mobile Ads 초기화 완료.");
            _isInitialized = true;
        });
    }

    #region App Open Ad
    public bool IsAppOpenAdAvailable =>
        _appOpenAd != null && (DateTime.UtcNow - _appOpenAdLoadTime) < TIMEOUT;

    public void LoadAppOpenAd()
    {
        if (BLOCK_ADS || IsAppOpenAdAvailable) return;

        Debug.Log("앱 오프닝 광고 로드 중...");
        var adRequest = new AdRequest();


        AppOpenAd.Load(GetAdUnitId(APP_OPEN_AD_TEST_ID, APP_OPEN_AD_ID), adRequest, (AppOpenAd ad, LoadAdError error) =>
            {
                if (error != null)
                {
                    Debug.LogError($"앱 오프닝 광고 로드 실패: {error}");
                    return;
                }

                _appOpenAd = ad;
                _appOpenAdLoadTime = DateTime.UtcNow;

                _appOpenAd.OnAdFullScreenContentClosed += () =>
                {
                    Debug.Log("앱 오프닝 광고 닫힘.");
                    LoadAppOpenAd(); // 닫힌 후 새로운 광고 로드
                };
            });
    }

    public void ShowAppOpenAd()
    {
        if (BLOCK_ADS || !IsAppOpenAdAvailable)
        {
            Debug.LogWarning("앱 오프닝 광고를 사용할 수 없습니다.");
            return;
        }

        Debug.Log("앱 오프닝 광고 표시.");
        _appOpenAd.Show();
    }

    #endregion


    public void DestroyAd()
    {
        if (_appOpenAd != null)
        {
            Debug.Log("Destroying app open ad.");
            _appOpenAd.Destroy();
            _appOpenAd = null;
        }
    }
}
