// Configure passive event listeners to improve scroll performance
// This fixes the browser warning about non-passive touchstart listeners

import { boot } from 'quasar/wrappers'

export default boot(() => {
  // Suppress browser extension errors
  const originalConsoleError = console.error
  console.error = (...args) => {
    const message = args.join(' ')
    // Filter out MetaMask and browser extension errors
    if (message.includes('MetaMask') ||
        message.includes('chrome-extension') ||
        message.includes('message channel closed') ||
        message.includes('Host validation failed') ||
        message.includes('Host is not supported')) {
      return // Suppress these errors
    }
    originalConsoleError.apply(console, args)
  }

  // Override addEventListener to make touch events passive by default
  const originalAddEventListener = EventTarget.prototype.addEventListener

  EventTarget.prototype.addEventListener = function (type, listener, options) {
    // For touch events, make them passive by default if not explicitly set
    if (['touchstart', 'touchmove', 'wheel', 'mousewheel'].includes(type)) {
      if (typeof options === 'boolean') {
        // Convert boolean to options object
        options = { capture: options, passive: true }
      } else if (typeof options === 'object' && options !== null) {
        // Add passive: true if not explicitly set
        if (options.passive === undefined) {
          options = { ...options, passive: true }
        }
      } else {
        // Default case - make it passive
        options = { passive: true }
      }
    }

    return originalAddEventListener.call(this, type, listener, options)
  }

  // Add global error handler for unhandled promise rejections
  window.addEventListener('unhandledrejection', (event) => {
    const message = event.reason?.message || event.reason
    if (message && (
      message.includes('MetaMask') ||
      message.includes('chrome-extension') ||
      message.includes('message channel closed') ||
      message.includes('Host validation failed') ||
      message.includes('Host is not supported')
    )) {
      event.preventDefault() // Prevent the error from showing in console
    }
  })
})
