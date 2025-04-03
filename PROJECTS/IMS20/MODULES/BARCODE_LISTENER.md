[:arrow_up: IMS20](/PROJECTS/IMS20/IMS20.md)

# Barcode Listener
Class that captures barcode reader signals in *WinFroms* environment.

## SOURCE CODE FILES
:link: [BarcodeListener.cs](/PROJECTS/IMS20/SOURCES/BarcodeListener.cs)

# Initialization
``` csharp
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
}
```
###### Code @ BarcodeListener.cs (fragment)
Once a **Barcode Listener** object is created, it subscribes to the form’s keypress event and starts listening for input from the barcode scanner.

To correctly differentiate between normal input and barcode scanner signal I use *System.Windows.Forms.Timer*.
``` csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BarcodeListener : IDisposable
{    
    private const int MinimumBarcodeLength = 10;
    public event EventHandler<string> BarcodeScanned;

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
}
```
###### Code @ BarcodeListener.cs (fragment)

To use **Barcode Listener** of form one need to create object and subscribes its *BarcodeScanned* event:
```csharp
//Barcode Listener
this.barcodeListener = new BarcodeListener(this);
this.barcodeListener.BarcodeScanned += BarcodeListener_BarcodeScanned;
```