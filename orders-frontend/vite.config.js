import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

export default defineConfig({
  plugins: [vue()],
  base: './',
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
        target: 'https://localhost:5001', // adjust to your backend URL
        changeOrigin: true,
        secure: false
      }
    }
  }
})
