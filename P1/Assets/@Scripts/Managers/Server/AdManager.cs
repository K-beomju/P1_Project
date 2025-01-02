using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdManager
{
    private bool _isInitialized;

    private AppOpenAd _appOpenAd;
    private RewardedInterstitialAd _rewardedInterstitialAd;
    private BannerView _bannerView;

    private const string REWARDED_AD_TEST_ID = "ca-app-pub-3940256099942544/5354046379";
    private const string BANNER_AD_TEST_ID = "ca-app-pub-3940256099942544/6300978111";

    private const string REWARDED_AD_ID = "ca-app-pub-1077488135922668/6986804114"; // 실제 보상형 광고 ID
    private const string BANNER_AD_ID = "ca-app-pub-1077488135922668/8425080691"; // 실제 배너 광고 ID

    private bool TEST_MODE = true; // 테스트 모드 활성화 여부
    private bool BLOCK_ADS = false; // 광고 차단 여부

    internal static List<string> TestDeviceIds = new List<string>()
    {
        AdRequest.TestDeviceSimulator,
        #if UNITY_IPHONE
        "96e23e80653bb28980d3f40beb58915c",
        #elif UNITY_ANDROID
        "9faf2ee8-c8d7-4339-8374-2eb065ab0b46"
        #endif
    };

    private string GetAdUnitId(string testId, string liveId) => TEST_MODE ? testId : liveId;

    public void Init()
    {
#if UNITY_EDITOR
Debug.Log("테스트 모드 활성화");
        TEST_MODE = true;
#elif UNITY_ANDROID
Debug.Log("테스트 모드 비활성화");
        TEST_MODE = false;
#endif

        InitializeAds();
    }

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

            LoadRewardedInterstitialAd();
        });
    }



    #region Rewarded Interstitial Ad
    public void LoadRewardedInterstitialAd(Action<bool> onAdLoaded = null)
    {
        // 이전 광고 제거
        if (_rewardedInterstitialAd != null)
        {
            DestroyAd();
        }

        Debug.Log("보상형 전면 광고 로드 중...");
        var adRequest = new AdRequest();

        RewardedInterstitialAd.Load(GetAdUnitId(REWARDED_AD_TEST_ID, REWARDED_AD_ID), adRequest,
            (RewardedInterstitialAd ad, LoadAdError error) =>
            {
                if (error != null)
                {
                    Debug.LogError($"보상형 전면 광고 로드 실패: {error}");
                    onAdLoaded?.Invoke(false);
                    return;
                }

                if (ad == null)
                {
                    Debug.LogError("예기치 않은 오류: 보상형 전면 광고 로드 시 null 반환.");
                    onAdLoaded?.Invoke(false);
                    return;
                }

                Debug.Log($"보상형 전면 광고 로드 성공: {ad.GetResponseInfo()}");
                _rewardedInterstitialAd = ad;

                // 이벤트 핸들러 등록 (Null 확인)
                if (_rewardedInterstitialAd != null)
                {
                    RegisterRewardedInterstitialAdEventHandlers(_rewardedInterstitialAd);
                }

                onAdLoaded?.Invoke(true);
            });
    }


    public void ShowRewardedInterstitialAd(Action<bool> onRewardEarned = null)
    {
        if (BLOCK_ADS || _rewardedInterstitialAd == null || !_rewardedInterstitialAd.CanShowAd())
        {
            Debug.LogWarning("광고를 표시할 수 없는 상태. 로드 시도 중...");
            LoadRewardedInterstitialAd(loaded =>
            {
                if (loaded && _rewardedInterstitialAd.CanShowAd())
                {
                    Debug.Log("보상형 전면 광고 재로드 성공. 광고 표시 시도...");
                    ShowRewardedInterstitialAd(onRewardEarned); // 재시도
                }
                else
                {
                    Debug.LogWarning("보상형 전면 광고 로드 실패 또는 표시 불가.");
                    onRewardEarned?.Invoke(false);
                }
            });
            return;
        }

        Debug.Log("보상형 전면 광고 표시.");
        try
        {
            _rewardedInterstitialAd.Show(reward =>
            {
                Debug.Log("광고 시청 완료. 보상 지급 처리...");
                Managers.Backend.GameData.QuestData.UpdateQuest(Define.EQuestType.WatchAds);
                onRewardEarned?.Invoke(true); // 보상 성공
            });
        }
        catch (Exception ex)
        {
            Debug.LogError($"광고 표시 중 예외 발생: {ex.Message}");
            onRewardEarned?.Invoke(false); // 예외 발생 시 실패 처리
        }
    }


    private void RegisterRewardedInterstitialAdEventHandlers(RewardedInterstitialAd ad)
    {
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log($"보상형 전면 광고 수익 발생: {adValue.Value} {adValue.CurrencyCode}");
        };

        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("보상형 전면 광고 노출 기록됨.");
        };

        ad.OnAdClicked += () =>
        {
            Debug.Log("보상형 전면 광고 클릭됨.");
        };

        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("보상형 전면 광고 전체 화면 콘텐츠 열림.");
        };

        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("보상형 전면 광고 닫힘.");
            LoadRewardedInterstitialAd(); // 닫힌 후 새로운 광고 로드
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError($"보상형 전면 광고 전체 화면 콘텐츠 열기 실패: {error}");
        };
    }
    #endregion

    #region Banner Ad
    public void LoadBannerAd()
    {
        if (BLOCK_ADS) return;

        // 배너 광고 초기화
        if (_bannerView != null)
        {
            _bannerView.Destroy();
        }

        _bannerView = new BannerView(GetAdUnitId(BANNER_AD_TEST_ID, BANNER_AD_ID), AdSize.Banner, AdPosition.Top);

        var adRequest = new AdRequest();

        _bannerView.LoadAd(adRequest);

        // 배너 광고 이벤트 핸들러
        _bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("배너 광고 로드 성공");
        };

        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError($"배너 광고 로드 실패: {error}");
        };

        _bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("배너 광고 열림");
        };

        _bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("배너 광고 닫힘");
        };
    }

    public void HideBannerAd()
    {
        _bannerView?.Hide();
    }

    public void ShowBannerAd()
    {
        _bannerView?.Show();
    }

    public void DestroyBannerAd()
    {
        _bannerView?.Destroy();
        _bannerView = null;
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

        if (_rewardedInterstitialAd != null)
        {
            Debug.Log("Destroying rewarded interstitial ad.");
            _rewardedInterstitialAd.Destroy();
            _rewardedInterstitialAd = null;
        }

        DestroyBannerAd();

    }

    public IEnumerator ExecuteAfterFrame(Action action)
    {
        yield return null; // 한 프레임 대기
        action?.Invoke(); // 전달된 액션 실행
    }
}
