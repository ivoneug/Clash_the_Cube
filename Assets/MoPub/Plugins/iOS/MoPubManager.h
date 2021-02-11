//
//  MopubManager.h
//  MoPub
//
//  Copyright (c) 2017 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#if __has_include(<MoPub/MoPub.h>)
#import <MoPub/MoPub.h>
#elif __has_include(<MoPubSDKFramework/MoPub.h>)
#import <MoPubSDKFramework/MoPub.h>
#else
#import "MoPub.h"
#endif

__deprecated_enum_msg("MoPubBannerType has been deprecated, please use MoPubAdSize instead.")
typedef enum
{
    MoPubBannerType_320x50,
    MoPubBannerType_300x250,
    MoPubBannerType_728x90,
    MoPubBannerType_160x600
} MoPubBannerType;

typedef enum
{
    MoPubAdSize_50Height,
    MoPubAdSize_90Height,
    MoPubAdSize_250Height,
    MoPubAdSize_280Height
} MoPubAdSize;

typedef enum
{
    MoPubAdPositionTopLeft,
    MoPubAdPositionTopCenter,
    MoPubAdPositionTopRight,
    MoPubAdPositionCentered,
    MoPubAdPositionBottomLeft,
    MoPubAdPositionBottomCenter,
    MoPubAdPositionBottomRight
} MoPubAdPosition;

typedef void (*MoPubBackgroundEventCallback)(const char* eventName, const char* eventArgsJson);

@interface MoPubManager : NSObject <MPAdViewDelegate, MPInterstitialAdControllerDelegate, CLLocationManagerDelegate, MPRewardedVideoDelegate>
{
@private
    BOOL _locationEnabled;
    NSString* _adUnitId;
    BOOL _autorefresh;
}
@property (class, nonatomic) MoPubBackgroundEventCallback bgEventCallback;
@property (nonatomic, strong) MPAdView* adView;
@property (nonatomic, strong) CLLocationManager* locationManager;
@property (nonatomic, strong) CLLocation* lastKnownLocation;
@property (nonatomic) MoPubAdPosition bannerPosition;

+ (MoPubManager*)sharedManager;

+ (MoPubManager*)managerForAdunit:(NSString*)adUnitId;

+ (UIViewController*)unityViewController;

+ (void)sendUnityEvent:(NSString*)eventName withArgs:(NSArray*)args backgroundOK:(BOOL)bg;

+ (void)sendUnityEvent:(NSString*)eventName withArgs:(NSArray*)args;

- (void)sendUnityEvent:(NSString*)eventName backgroundOK:(BOOL)bg;

- (void)sendUnityEvent:(NSString*)eventName;

- (id)initWithAdUnit:(NSString*)adUnitId;

- (void)enableLocationSupport:(BOOL)shouldEnable;

- (void)requestBanner:(float)width height:(float)height atPosition:(MoPubAdPosition)position keywords:(NSString*)keywords userDataKeywords:(NSString*)userDataKeywords;

__deprecated_msg("createBanner has been deprecated, please use requestBanner instead.");
- (void)createBanner:(MoPubBannerType)bannerType atPosition:(MoPubAdPosition)position;

- (void)destroyBanner;

- (void)showBanner;

- (void)hideBanner:(BOOL)shouldDestroy;

- (void)setAutorefreshEnabled:(BOOL)enabled;

- (void)forceRefresh;

- (void)refreshAd:(NSString*)keywords userDataKeywords:(NSString*)userDataKeywords;

- (void)requestInterstitialAd:(NSString*)keywords userDataKeywords:(NSString*)userDataKeywords;

- (BOOL)interstitialIsReady;

- (void)showInterstitialAd;

- (void)destroyInterstitialAd;

@end
