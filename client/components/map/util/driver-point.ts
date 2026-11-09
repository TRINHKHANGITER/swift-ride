import type { GeoJSONSource, Map, StyleImageInterface } from "maplibre-gl";

type PulsingDot = StyleImageInterface & {
  context: CanvasRenderingContext2D | null;
};

const size = 120;
const imageId = "client-location-pulsing-dot";
const sourceId = "client-location";
const layerId = "client-location";

const locationFeature = (location: { latitude: number; longitude: number }) => ({
  type: "FeatureCollection" as const,
  features: [
    {
      type: "Feature" as const,
      properties: {},
      geometry: {
        type: "Point" as const,
        coordinates: [location.longitude, location.latitude],
      },
    },
  ],
});

export const drawClientLocation = (map: Map, location: { latitude: number, longitude: number }) => {
  const existingSource = map.getSource(sourceId) as GeoJSONSource | undefined;
  if (existingSource) {
    existingSource.setData(locationFeature(location));
    return;
  }

  const pulsingDot: PulsingDot = {
    width: size,
    height: size,
    data: new Uint8Array(size * size * 4),
    context: null,

    // get rendering context for the map canvas when layer is added to the map
    onAdd() {
      const canvas = document.createElement("canvas");
      canvas.width = this.width;
      canvas.height = this.height;
      this.context = canvas.getContext("2d");
    },

    // called once before every frame where the icon will be used
    render() {
      const duration = 1000;
      const t = (performance.now() % duration) / duration;

      const radius = (size / 2) * 0.3;
      const outerRadius = (size / 2) * 1 * t + radius;
      const context = this.context;

      if (!context) {
        return false;
      }

      // draw outer circle
      context.clearRect(0, 0, this.width, this.height);
      context.beginPath();
      context.arc(this.width / 2, this.height / 2, outerRadius, 0, Math.PI * 2);
      context.fillStyle = `rgba(150, 200, 255,${1 - t})`;
      context.fill();

      // draw inner circle
      context.beginPath();
      context.arc(this.width / 2, this.height / 2, radius, 0, Math.PI * 2);
      context.fillStyle = "rgba(50, 50, 255, 1)";
      context.strokeStyle = "white";
      context.lineWidth = 2 + 4 * (1 - t);
      context.fill();
      context.stroke();

      // update this image's data with data from the canvas
      this.data = context.getImageData(0, 0, this.width, this.height).data;

      // continuously repaint the map, resulting in the smooth animation of the dot
      map.triggerRepaint();

      // return `true` to let the map know that the image was updated
      return true;
    },
  };

  const draw = () => {
    const source = map.getSource(sourceId) as GeoJSONSource | undefined;
    if (source) {
      source.setData(locationFeature(location));
      return;
    }

    if (!map.hasImage(imageId)) {
      map.addImage(imageId, pulsingDot, { pixelRatio: 2 });
    }

    map.addSource(sourceId, {
      type: "geojson",
      data: locationFeature(location),
    });

    map.addLayer({
      id: layerId,
      type: "symbol",
      source: sourceId,
      layout: {
        "icon-image": imageId,
      },
    });
  };

  if (map.loaded()) {
    draw();
  } else {
    map.once("load", draw);
  }
};
