[:arrow_up: ALTPOM](/PROJECTS/ALTPOM/ALTPOM.md)


# Dynamic Button Form
Pop up form, allowed to be initialized with any object collection, displaying buttons with text corresponding to certain property of an object. Stylized with use of HTML and CSS.

## SOURCE CODE FILES
:link: [ButtonDisplayer.cs](/PROJECTS/ALTPOM/SOURCE/ButtonDisplayer.cs)\
:link: [DynamicButtonForm.cs](/PROJECTS/ALTPOM/SOURCE/DynamicButtonForm.cs)\
:link: [DynamicButtonFormInitialization.cs](/PROJECTS/ALTPOM/SOURCE/DynamicButtonFormInitialization.cs)\
:link: [DynamicButtonForm.html](/PROJECTS/ALTPOM/SOURCE/DynamicButtonForm.html)\
:link: [DynamicButtonForm.css](/PROJECTS/ALTPOM/SOURCE/DynamicButtonForm.css)


# Dynamic Button Form (DBF)
To use **DBF** it firstly needs to be created and then initialized using public method *Initialize\<T\>*:
``` csharp
public partial class DynamicButtonForm : DevExpress.XtraEditors.XtraForm
{
    private object choosen;        

    #region Construction
    public DynamicButtonForm()
    {
        InitializeComponent();
    }

    public void Initilize<T>(List<T> sourceList_, string propertyName_, string caption_, 
                                int itemWidth_ = 200, int itemHeight_ = 80)
    {
        this.LoadSource<T>(sourceList_, propertyName_);
        this.lbCaption.Text = caption_;
        this.viewMain.OptionsTiles.ItemSize = new Size(itemWidth_, itemHeight_);            
    }        
    #endregion
}
```
###### Code @ DynamicButtonForm.cs (fragment)

*Initilize\<T\>* method takes following parameters:
- **sourceList_:** List of objects of type T
- **propertyName_:** Name of the property whose value will be displayed
- **caption_:** Text of label that is located on top
- **itemWidth_:** Width of buttons
- **itemHeight:** Height of buttons
  
**DBF** also has public methods:
``` csharp
public partial class DynamicButtonForm : DevExpress.XtraEditors.XtraForm
{
    private object choosen;        

    #region Methods
    #region Public        
    public void Select<T>(T instance, string display=default)
    {
        if (instance == null && display == default)
            return;

        List<ButtonDisplayer<T>> list = this.gridMain.DataSource as List<ButtonDisplayer<T>>;
        int index = list.FindIndex(x => EqualityComparer<T>.Default.Equals(x.ObjectData, instance) || x.PropertyToDisplay==display);
        this.Select(index);
    }

    public void Select(int index)
    {     
        viewMain.FocusedRowHandle = index;
    }

    public T GetChoosen<T>()
    {
        ButtonDisplayer<T> s = this.choosen as ButtonDisplayer<T>;
        if (s == null)
            return default;
        else
            return s.ObjectData;
    }
    #endregion
    #endregion
}
```
###### Code @ DynamicButtonForm.cs (fragment)

- **Select** method allows to choose certain button via code 
- **GetChoosen\<T\>** returns object whose button was double clicked



# Initialization of DBF
Here is simple example on how to use **DBF**:
``` csharp
private class CodeSample
{
    public string Code { get; set; }
    public string SourceFile { get; set; }
    public string ProjectName { get; set; }
}

private void InitializeDBF()
{
    List<CodeSample> codeSamples = GetCodeSamples();
    DynamicButtonForm dbf = new DynamicButtonForm();
    dbf.Initilize<CodeSample>(codeSamples, nameof(CodeSample.Code), "Choose: Code Sample", itemWidth_: 300);
    if (dbf.ShowDialog() == DialogResult.OK)
    {
        CodeSample code = dbf.GetChoosen<CodeSample>();
        if (code != null)
        {
            Console.WriteLine($"{code?.Code} from {code?.ProjectName} in {code?.SourceFile}");
        }
    }
}
```
I firstly create **DBF** instance and then call Initialize method with correct parameters values.

This is end result:
![Image @ DynamicButtonForm.png](/PROJECTS/ALTPOM/SOURCE/DynamicButtonForm.png)

###### Code @ DynamicButtonFormInitialization.cs (fragment)

# Buttons visualization
Buttons are visualized with use of HTML and CSS:
``` HTML
<div class="container">		
	<div class="item" >
		<div class="item-section">
			<div class="label">
				${PropertyToDisplay}
			</div>	
		</div>		
	</div>
</div>
```
###### Code @ DynamicButtonForm.html

``` CSS
/* Container */

.container {
    width: 100%;
    height: 100%; 
    display: flex;      
}
  
.container:hover .item
{    
    color: @Control;
    background-color: @ControlText;
    border-color: @HighlightAlternate;
    box-shadow: 10px 10px 10px @HighlightAlternate;
    margin-top: 0px;
    margin-bottom: 20px;
    margin-left: 0px;
    margin-right: 20px;
    color: rgb(0, 0, 0);
    background-color: rgb(255, 255, 255);
}

.container:hover .item .item-section
{	  
    background-color: rgb(255, 255, 255);
}

.container:active .item
{	
    color: @HighlightAlternate;  
}	

/* Item and section */

.item {  
    position: relative;
    width: 100%;
    margin: 10px;
    display: flex;
    padding: 5px;
    border: 5px solid @HighlightAlternate;
    border-radius: 15px;
    color: @ControlText;
    background-color: @Control;  
}

.item-section
{
    background-color: @ControlText/0.1;
    width: 100%;	
    padding: 5px;
    border-radius: 15px;
    display: flex;
    justify-content: center;
    align-content: center;	    
    overflow: visible;
    overflow-wrap: anywhere;
}

.label
{
    display: flex;
    font-size: 20px;	
}

/* Button Selection */

:root:select .item
{    
    color: @Red;    
    border-color: @Red;
    box-shadow: 10px 10px 10px @Red;
    margin-top: 0px;
    margin-bottom: 20px;
    margin-left: 0px;
    margin-right: 20px;        
    background-color: rgb(255, 255, 255);
}

:root:select .item .item-section
{	  
    background-color: rgb(255, 255, 255);
}

:root:select .item:hover
{
    color: @Red;    
    border-color: @Red;
    box-shadow: 10px 10px 10px @Red;
    margin-top: 0px;
    margin-bottom: 20px;
    margin-left: 0px;
    margin-right: 20px;
    background-color: rgb(255, 255, 255);
}
```
###### Code @ DynamicButtonForm.css