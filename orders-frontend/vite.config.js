import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue()],
  build: {
    cssCodeSplit: false,
    rollupOptions: {
      output: {
        manualChunks: undefined
      }
    }
  },
  server: {
    port: 5173,
    proxy: {
      // proxy API calls to your ASP.NET backend
      '/api': {
        target: 'http://localhost:5054', // adjust to your backend URL
        changeOrigin: true,
        secure: false
      }
    }
  }
})
