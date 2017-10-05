var selected = Cookies.get('selected');
var SelectedRenameFolderName;




function mailContextMenuSelect(e) {
    
    $('.menutxtFoldername').css("display", "none");
    $('.menudivFoldername').css("display", "");
    $("#DivAddFolderFromMenu").css("display", "none");    
    switch (e.item.getAttribute("operation")) {
        case "MenuDeleteFolder":
            var URl = APIBaseUrl+'/api/ApiHome/DeleteFolder';            
            if ($(e.target).find("#navigationTreeView_tv_active").find('.MenuFolderid').html()) {
                if (confirm('Are you Sure to delete Folder')) {
                    $.ajax({
                        url: URl,
                        cache: false,
                        async: false,
                        data: { Id: $(e.target).find("#navigationTreeView_tv_active").find('.MenuFolderid').html() },
                        success: function (EditContent) {
                            GetFoldersByMailBoxId(GridDataInAjaxCall, null);
                            showSuccessNotification("Folder Deleted");
                        },
                        error: function (error) {
                            alert('Error');
                        }
                    })
                }
            }
            else {
                alert("Folder not selectd for delete");
            }
            
            break;
        case "MenuRenameFolder":
            if ($(e.target).find("#navigationTreeView_tv_active").find('.menutxtFoldername').text())
            {
                SelectedRenameFolderName = $(e.target).find("#navigationTreeView_tv_active").find('.menutxtFoldername').text();
                $(e.target).find("#navigationTreeView_tv_active").find('.menutxtFoldername').css("display", "");
                $(e.target).find("#navigationTreeView_tv_active").find('.menutxtFoldername').focus();
                $(e.target).find("#navigationTreeView_tv_active").find('.menudivFoldername').css("display", "none");
            }
            else {
                alert("Folder not exsit for rename");
            }
             
            break;
        case "MenuSetDefoultFolder":
           var SelectedFolder = $(e.target).find("#navigationTreeView_tv_active").find('.menutxtFoldername').text();
           var r = confirm("Are you sure to set defoult folder to " + SelectedFolder);
          
           var URl = APIBaseUrl + '/api/ApiHome/SetDefoultFolder';
           if (r == true) {
               $.ajax({
                   url: URl,
                   cache: false,
                   async: false,
                   data: { FolderId: $(e.target).find("#navigationTreeView_tv_active").find('.MenuFolderid').html(), SelectedMailBoxId:$("#ListOfMailBox").val() },
                   success: function (EditContent) {
                       GetFoldersByMailBoxId(GridDataInAjaxCall, null);
                       showSuccessNotification("Defoult Folder set to " + SelectedFolder);
                   },
                   error: function (error) {
                       alert('Error');
                   }
               })
            } else {
                txt = "You pressed Cancel!";
            }
            break;

    }
}
jQuery('body').on('focusout', '.TxtAddFolderFromMenu,.menutxtFoldername', function (event) {
   

    if ($(this).attr('class').indexOf("TxtAddFolderFromMenu") > -1)
    {
        if ($("#TxtAddFolderFromMenu").val()) {

            if ($("#ListOfMailBox").val()) {
                var Foldermodel = {};
                Foldermodel.Id = 0;
                Foldermodel.Name = $(this).val();
                Foldermodel.MailBoxId = $("#ListOfMailBox").val();
                Foldermodel.TypeId = 1;
                $.ajax({
                    url: APIBaseUrl + 'api/ApiHome/AddEditFolder',
                    type: "POST",
                    cache: false,
                    data: Foldermodel,
                    success: function (Response) {
                        if (Response == true) {
                            GetFoldersByMailBoxId(GridDataInAjaxCall, null);
                            showSuccessNotification("Folder Added Sucesfully");
                        }
                        else {
                            alert("Error ");
                        }
                    },
                    error: function (error) {
                        //  alert('Error');
                    }
                })
            }
            else {
                alert("No mailbox selected")
            }
        }
    }
    else {       
        if (SelectedRenameFolderName.toLowerCase() != $(this).val().toLowerCase()) {
            var Foldermodel={};            
            Foldermodel.Id=$(this).closest('div').find('.MenuFolderid').html();
            Foldermodel.Name= $(this).val();
            Foldermodel.MailBoxId= $("#ListOfMailBox").val();
            Foldermodel.TypeId = 1;
            $.ajax({
                url: APIBaseUrl + 'api/ApiHome/AddEditFolder',
                type:"POST",
                cache: false,               
                data: Foldermodel,
                success: function (Response) {              
                    if (Response == true) {
                        GetFoldersByMailBoxId(GridDataInAjaxCall, null);
                        showSuccessNotification("Folder Rename Sucesfully");
                    }
                    else {
                        alert("Error ");
                    }
                },
                error: function (error) {
                    //  alert('Error');
                }
            })
        }
       
    }
    $('.menutxtFoldername').css("display", "none");
    $('.menudivFoldername').css("display", "");
    $("#DivAddFolderFromMenu").css("display", "none");
    
});


function mailContextMenuOpen(e) {
    $('.menutxtFoldername').css("display", "none");
    $('.menudivFoldername').css("display", "");
    $("#DivAddFolderFromMenu").css("display", "none");
}


function setSelectedNode(selected) {
    var treeview = $("#navigationTreeView").data("kendoTreeView");
    if (selected) {
        Cookies.remove('selected');
        var node = $("#navigationTreeView").find('li').eq(selected);
        treeview.select(node);
    }
}

function populateNavigationTree(data, FromSearchFolder) {
    //console.log(data);
   
    var newDataSource = new kendo.data.HierarchicalDataSource({ data: data });
    var navigationTreeView = $('#navigationTreeView').data('kendoTreeView');
    navigationTreeView.setDataSource(newDataSource);
    if (FromSearchFolder == "FromSearchFolder") {
        setSelectedNode("");
    }
    else {
        if (selected) {
            if (Cookies.get('ChangedIdOfFolder') == "0") {
                setSelectedNode("0");
            }
            else {
                setSelectedNode(selected);
            }
        }
        else {
            setSelectedNode("0");
        }
    }
}