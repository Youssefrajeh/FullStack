// Configuration for your app
// https://v2.quasar.dev/quasar-cli-webpack/quasar-config-file

import { defineConfig } from '#q-app/wrappers'

export default defineConfig(({ dev }) => {
  return {
    eslint: {
      warnings: true,
      errors: true
    },

    boot: [
      'passive-events'
    ],

    css: [
      'app.scss'
    ],

    extras: [
      'roboto-font',
      'material-icons',
    ],

    build: {
      vueRouterMode: 'hash',
      esbuildTarget: {
        browser: [ 'es2022', 'firefox115', 'chrome115', 'safari14' ],
        node: 'node20'
      },
    },

    devServer: {
      server: {
        type: 'http'
      },
      open: true
    },

    framework: {
      config: {},
      plugins: []
    },

    animations: [],

    ssr: {
      prodPort: 3000,
      middlewares: [
        'render'
      ],
      pwa: false
    },

    pwa: false,

    cordova: {
    },

    capacitor: {
      hideSplashscreen: true
    },

    electron: {
      preloadScripts: [ 'electron-preload' ],
      inspectPort: 5858,
      bundler: 'packager',

      packager: {
      },

      builder: {
        appId: 'casestudy01'
      }
    },

    bex: {
      extraScripts: []
    }
  }
})
