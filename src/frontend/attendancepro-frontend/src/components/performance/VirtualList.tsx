import React, { useState, useRef, useMemo } from 'react';

interface VirtualListProps<T> {
  items: T[];
  itemHeight: number;
  containerHeight: number;
  renderItem: (item: T, index: number) => React.ReactNode;
  overscan?: number;
  className?: string;
}

export function VirtualList<T>({
  items,
  itemHeight,
  containerHeight,
  renderItem,
  overscan = 5,
  className = ''
}: VirtualListProps<T>) {
  const [scrollTop, setScrollTop] = useState(0);
  const containerRef = useRef<HTMLDivElement>(null);

  const visibleRange = useMemo(() => {
    const visibleCount = Math.ceil(containerHeight / itemHeight);
    const startIndex = Math.max(0, Math.floor(scrollTop / itemHeight) - overscan);
    const endIndex = Math.min(items.length - 1, startIndex + visibleCount + overscan * 2);
    
    return { startIndex, endIndex, visibleCount };
  }, [scrollTop, itemHeight, containerHeight, items.length, overscan]);

  const totalHeight = items.length * itemHeight;
  const offsetY = visibleRange.startIndex * itemHeight;

  const handleScroll = (e: React.UIEvent<HTMLDivElement>) => {
    setScrollTop(e.currentTarget.scrollTop);
  };

  const visibleItems = useMemo(() => {
    return items.slice(visibleRange.startIndex, visibleRange.endIndex + 1);
  }, [items, visibleRange.startIndex, visibleRange.endIndex]);

  return (
    <div
      ref={containerRef}
      className={`overflow-auto ${className}`}
      style={{ height: containerHeight }}
      onScroll={handleScroll}
    >
      <div style={{ height: totalHeight, position: 'relative' }}>
        <div
          style={{
            transform: `translateY(${offsetY}px)`,
            position: 'absolute',
            top: 0,
            left: 0,
            right: 0
          }}
        >
          {visibleItems.map((item, index) => (
            <div
              key={visibleRange.startIndex + index}
              style={{ height: itemHeight }}
            >
              {renderItem(item, visibleRange.startIndex + index)}
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}

interface VirtualGridProps<T> {
  items: T[];
  itemWidth: number;
  itemHeight: number;
  containerWidth: number;
  containerHeight: number;
  renderItem: (item: T, index: number) => React.ReactNode;
  gap?: number;
  className?: string;
}

export function VirtualGrid<T>({
  items,
  itemWidth,
  itemHeight,
  containerWidth,
  containerHeight,
  renderItem,
  gap = 0,
  className = ''
}: VirtualGridProps<T>) {
  const [scrollTop, setScrollTop] = useState(0);
  const containerRef = useRef<HTMLDivElement>(null);

  const columnsPerRow = Math.floor((containerWidth + gap) / (itemWidth + gap));
  const totalRows = Math.ceil(items.length / columnsPerRow);
  const rowHeight = itemHeight + gap;

  const visibleRange = useMemo(() => {
    const visibleRows = Math.ceil(containerHeight / rowHeight);
    const startRow = Math.max(0, Math.floor(scrollTop / rowHeight) - 1);
    const endRow = Math.min(totalRows - 1, startRow + visibleRows + 1);
    
    const startIndex = startRow * columnsPerRow;
    const endIndex = Math.min(items.length - 1, (endRow + 1) * columnsPerRow - 1);
    
    return { startRow, endRow, startIndex, endIndex };
  }, [scrollTop, rowHeight, totalRows, columnsPerRow, items.length]);

  const totalHeight = totalRows * rowHeight;
  const offsetY = visibleRange.startRow * rowHeight;

  const handleScroll = (e: React.UIEvent<HTMLDivElement>) => {
    setScrollTop(e.currentTarget.scrollTop);
  };

  const visibleItems = useMemo(() => {
    return items.slice(visibleRange.startIndex, visibleRange.endIndex + 1);
  }, [items, visibleRange.startIndex, visibleRange.endIndex]);

  return (
    <div
      ref={containerRef}
      className={`overflow-auto ${className}`}
      style={{ height: containerHeight }}
      onScroll={handleScroll}
    >
      <div style={{ height: totalHeight, position: 'relative' }}>
        <div
          style={{
            transform: `translateY(${offsetY}px)`,
            position: 'absolute',
            top: 0,
            left: 0,
            right: 0,
            display: 'grid',
            gridTemplateColumns: `repeat(${columnsPerRow}, ${itemWidth}px)`,
            gap: `${gap}px`
          }}
        >
          {visibleItems.map((item, index) => (
            <div key={visibleRange.startIndex + index}>
              {renderItem(item, visibleRange.startIndex + index)}
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
