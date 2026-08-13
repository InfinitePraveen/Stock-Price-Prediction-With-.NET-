namespace StockPricePredictor.Services;

public sealed class RidgePredictor
{
    private double[] _mean = Array.Empty<double>(), _std = Array.Empty<double>(), _w = Array.Empty<double>();

    public void Fit(List<double[]> X, List<double> y, double alpha = 1.0)
    {
        int n = X.Count, f = X[0].Length;
        _mean = new double[f];
        _std = new double[f];

        for (int j = 0; j < f; j++)
        {
            _mean[j] = X.Average(r => r[j]);
            _std[j] = Math.Sqrt(X.Average(r => Math.Pow(r[j] - _mean[j], 2)));
            if (_std[j] < 1e-12) _std[j] = 1;
        }

        var Z = X.Select(r => new[] { 1.0 }.Concat(r.Select((v,j) => (v-_mean[j])/_std[j])).ToArray()).ToArray();
        int d = f + 1;
        var A = new double[d,d];
        var b = new double[d];

        for (int i=0;i<n;i++)
        {
            for(int j=0;j<d;j++)
            {
                b[j] += Z[i][j] * y[i];
                for(int k=0;k<d;k++) A[j,k] += Z[i][j] * Z[i][k];
            }
        }
        for(int j=1;j<d;j++) A[j,j] += alpha;
        _w = Solve(A,b);
    }

    public double Predict(double[] x)
    {
        double sum = _w[0];
        for(int j=0;j<x.Length;j++) sum += _w[j+1] * ((x[j]-_mean[j])/_std[j]);
        return sum;
    }

    private static double[] Solve(double[,] a, double[] b)
    {
        int n=b.Length;
        var m=new double[n,n+1];
        for(int i=0;i<n;i++){for(int j=0;j<n;j++)m[i,j]=a[i,j];m[i,n]=b[i];}
        for(int c=0;c<n;c++)
        {
            int pivot=c;
            for(int r=c+1;r<n;r++) if(Math.Abs(m[r,c])>Math.Abs(m[pivot,c])) pivot=r;
            for(int j=c;j<=n;j++){var t=m[c,j];m[c,j]=m[pivot,j];m[pivot,j]=t;}
            double q=m[c,c]; if(Math.Abs(q)<1e-12) q=1e-12;
            for(int j=c;j<=n;j++)m[c,j]/=q;
            for(int r=0;r<n;r++) if(r!=c)
            {
                double f=m[r,c];
                for(int j=c;j<=n;j++)m[r,j]-=f*m[c,j];
            }
        }
        return Enumerable.Range(0,n).Select(i=>m[i,n]).ToArray();
    }
}
