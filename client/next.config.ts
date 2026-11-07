import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  /* config options here */
  cacheComponents: true,
  partialPrefetching: true,
  images: {
    remotePatterns: [
      {
        protocol: "https",
        hostname: "vietqr.app",
        pathname: "/img",
      },
    ],
  },
  turbopack: {
    rules: {
      "*.css": {
        loaders: ["@tailwindcss/turbopack"],
        as: "*.css",
      },
    },
  },
  // allowedDevOrigins: ["192.168.1.90", "livable-elsewhere-elyse.ngrok-free.dev"],
  reactStrictMode: false,
};

export default nextConfig;
