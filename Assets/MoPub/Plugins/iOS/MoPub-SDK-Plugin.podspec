Pod::Spec.new do |s|
  s.name                  = 'MoPub-SDK-Plugin'
  s.version               = '5.16.1'
  s.summary               = 'Unity wrapper for MoPub iOS SDK'
  s.homepage              = 'https://github.com/mopub/mopub-unity-sdk'
  s.license               = { :type => 'MoPub', :file => 'LICENSE' }
  s.author                = { 'MoPub' => 'support@mopub.com' }
  s.ios.deployment_target = '9.0'
  s.source                = { :git => 'https://github.com/mopub/mopub-unity-sdk.git', :tag => "v#{s.version}" }
  s.source_files          = '*.{h,m,mm,swift}'
  s.swift_version         = '5.0'
end
