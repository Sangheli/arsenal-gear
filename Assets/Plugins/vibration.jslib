mergeInto(LibraryManager.library, {
  Vibrate: function(duration) {
    if (typeof navigator !== 'undefined' && navigator.vibrate) {
      navigator.vibrate(duration);
    }
  }
}); 