export type InsertFn = (url: string, alt: string, href: string) => void;

export type VideoElement = SlateElement & {
  src: string;
  poster?: string;
};

export type Props = {
  height?: number | string;
  uploadPath?: string | null;
};
