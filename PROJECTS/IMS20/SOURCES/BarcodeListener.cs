using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BarcodeListener : IDisposable
{
    private System.Windows.Forms.Timer _timer;
    private string _barcode;
    private readonly Form _form;
    private const int BarcodeTimeout = 5000;
    private const int MinimumBarcodeLength = 10;
    public event EventHandler<string> BarcodeScanned;

    public BarcodeListener(Form form)
    {
        _form = form;
        _barcode = string.Empty;

        _timer = new System.Windows.Forms.Timer
        {
            Interval = BarcodeTimeout
        };
        _timer.Tick += TimerElapsed;
        _timer.Start();

        _form.KeyPreview = true;
        _form.KeyPress += Form_KeyPress;
    }

    private void Form_KeyPress(object sender, KeyPressEventArgs e)
    {            
        _timer.Stop();
        _timer.Start();
        _barcode += e.KeyChar;
        if (e.KeyChar == '\r' || e.KeyChar == '\n' 
            || e.KeyChar == Convert.ToChar(10) || e.KeyChar == Convert.ToChar(13))
        {
            if (_barcode.Length >= MinimumBarcodeLength)
            {
                OnBarcodeScanned(_barcode.Trim());
            }
            _barcode = string.Empty;
        }
    }

    private void TimerElapsed(object sender, EventArgs e)
    {            
        _barcode = string.Empty;
    }

    protected virtual void OnBarcodeScanned(string barcode)
    {
        BarcodeScanned?.Invoke(this, barcode);
    }

    public void Dispose()
    {
        if (_timer != null)
        {
            _timer.Stop();
            _timer.Tick -= TimerElapsed;
            _timer.Dispose();
            _timer = null;
        }
    }
}