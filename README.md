HybridWebView Custom Renderer
=============================

This fork enhances David Britch's original sample here:  [Implementing a HybridWebView](http://developer.xamarin.com/guides/cross-platform/xamarin-forms/custom-renderer/hybridwebview/).

David's code demonstrated a cutom renderer for a 'HybridWebView' custom control, to enhance the platform-specific web controls to allow C# code to be invoked from JavaScript.

It has been enhanced to allow certain features for Android and iOS


Android
-------
Enabling Location Services and Camera permissions in the WebView
Directly download PDFs and other files to local storage
Properly route mailto: , tel: , sms: links
Properly route Google Maps links
Route external links to a new system browser
Route internal links in the webview
Enable 1-touch enabling of Location Services
Splash Screen
Toast messages

iOS
-------
Enabling Location Services and Camera permissions in the WebView
Directly download PDFs and other files to local storage (not yet)
Properly route mailto: , tel: , sms: links
Properly route Google Maps links
Route external links to a new system browser
Route internal links in the webview
Toast messages


TO DO
--------
Enable iOS PDF downloads
Make Android's GPS detection modal --- don't load the website until they confirm OK.
Make the website to load a constant (currently loads fixity.io)
Check and test the Javascript bridge on both Android and iOS
Ensure 2-way communication between Native app and Web app via JS bridge
Let the JS bridge handle multiple actions (for example, Toast and Messagebox on native platforms)
Enable smart-pick of Apple Maps vs Google Maps on iOS for map links, depending on what is installed
Create Windows Universal App (code not modified at all)


Fork Author
------------------
Andrew Niese
Fresh Software LLC

Original Author
------------------
David Britch
