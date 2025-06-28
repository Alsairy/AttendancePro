import React, { Suspense, ComponentType } from 'react';
import LoadingSpinner from '../LoadingSpinner';

interface LazyLoadWrapperProps {
  children: React.ReactNode;
  fallback?: React.ComponentType;
  errorBoundary?: boolean;
}

export const LazyLoadWrapper: React.FC<LazyLoadWrapperProps> = ({
  children,
  fallback: Fallback = LoadingSpinner,
  errorBoundary = true
}) => {
  const content = (
    <Suspense fallback={<Fallback />}>
      {children}
    </Suspense>
  );

  if (errorBoundary) {
    return (
      <ErrorBoundary>
        {content}
      </ErrorBoundary>
    );
  }

  return content;
};

class ErrorBoundary extends React.Component<
  { children: React.ReactNode },
  { hasError: boolean; error?: Error }
> {
  constructor(props: { children: React.ReactNode }) {
    super(props);
    this.state = { hasError: false };
  }

  static getDerivedStateFromError(error: Error) {
    return { hasError: true, error };
  }

  componentDidCatch(error: Error, errorInfo: React.ErrorInfo) {
    console.error('LazyLoadWrapper Error:', error, errorInfo);
  }

  render() {
    if (this.state.hasError) {
      return (
        <div className="flex items-center justify-center p-8 text-center">
          <div className="max-w-md">
            <h3 className="text-lg font-semibold text-red-600 mb-2">
              Component Failed to Load
            </h3>
            <p className="text-gray-600 mb-4">
              There was an error loading this component. Please try refreshing the page.
            </p>
            <button
              onClick={() => window.location.reload()}
              className="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600"
            >
              Refresh Page
            </button>
          </div>
        </div>
      );
    }

    return this.props.children;
  }
}

export const withLazyLoading = <P extends object>(
  Component: ComponentType<P>,
  fallback?: ComponentType
) => {
  const WrappedComponent = React.forwardRef<any, P>((props, ref) => (
    <LazyLoadWrapper fallback={fallback}>
      <Component {...(props as any)} ref={ref} />
    </LazyLoadWrapper>
  ));
  
  WrappedComponent.displayName = `withLazyLoading(${Component.displayName || Component.name})`;
  return WrappedComponent;
};
