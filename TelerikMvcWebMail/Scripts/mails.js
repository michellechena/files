var navigated = false;
var marked = false;
var markedAsUnread = false;
var savedScroll = 0;
var AfterDisabled = 0;
var OtherOwner = 'NO';
var SelectedRowsForMove = "";
var TotalSelectedRecordForMove = 0;
$(document).ready(function () {
    // Preselect Mails category
    var treeview = $("#navigationTreeView").data("kendoTreeView");
    if (!Cookies.get('selected')) {
        Cookies.remove('selectedNodeText');
        setMenuItemsAvailability(false, "noselection");
    }

    if (Cookies.get('selectedNodeText')) {
        //  filterGrid(Cookies.get('selectedNodeText'));
    }
    else if (treeview.select().length == 0) {
        filterGrid($("#txtFirstFolderId").val());
    }

    $('.new-Mail').on('click', function (e) {
        $(".main-section").load(baseUrl + "/Home/NewMail");
        $(".main-section").removeClass("horizontal").removeClass("vertical");
    });
    $('#MenuNewMail').on('click', function (e) {
        $("#PartalViewContet").load(baseUrl + "/File/GetNewEmail");
        $("#PartalViewContet").css("display", "");
        $("#mainViewContent").css("display", "none");
        $("#IsParialViewLoaded").val("1");
        $(".main-section").removeClass("horizontal").removeClass("vertical");
    });
    $('#MenuUploadFile').on('click', function (e) {
        $("#PartalViewContet").load(baseUrl + "/File/UploadFile");
        $("#PartalViewContet").css("display", "");
        $("#mainViewContent").css("display", "none");
        $("#IsParialViewLoaded").val("1");
        $(".main-section").removeClass("horizontal").removeClass("vertical");
    });

    // Attach new mails handler
    $("#mainWidget").on("mousedown", "tr[role='row']", function (e) {
        if (e.which === 3) {
            if (!$(this).hasClass("k-state-selected")) {
                $("#mainWidget tbody tr").removeClass("k-state-selected");
                var mailGrid = $("#mainWidget").data("kendoGrid");
                mailGrid.select($(this));
            }
        }
    });

    // Attach search textbox handler
    $(".search-textbox").on('keyup', function (e) {

        var text = $(e.target).val().toLowerCase();
        var mailGrid = $("#mainWidget").data("kendoGrid");

        var dataInView = mailGrid.dataSource.view();
        dataInView.forEach(function (item) {
            var row = $('tr[data-uid="' + item.uid + '"]');
            if (item.Name.toLowerCase().indexOf(text) > -1 || item.Path.toLowerCase().indexOf(text) > -1) {
                row.show();
            } else {
                row.hide();
            }

        });
    });

    // Attach master checkbox handler
    $('.master-checkbox').on('change', function (e) {
        var grid = $("#mainWidget").data("kendoGrid");
        var dataInView = grid.dataSource.view();
        if (dataInView.length != 0) {
            var checked = e.target.checked;
            var grid = $("#mainWidget").data("kendoGrid");

            if (checked) {
                grid.select('tr');
                if (dataInView.length == 1) {
                    setMenuItemsAvailability(true);
                }
                else {
                    setMenuItemsAvailability(false, "multiselection");
                }
            } else {
                grid.clearSelection();
                setMenuItemsAvailability(true);
                setMenuItemsAvailability(false, "noselection");
            }
        }
    });
});



// Menu select item handler and its functions
function mailMenuSelect(e) {


    switch (e.item.getAttribute("operation")) {

        case "reply":
            mailReply(e.item.id);
            break;
        case "forward":
            mailForward(e.item.id);
            break;
        case "moveDelete":
            mailMoveDelete(e.item.id);
            break;
        case "moveDisabled":
            AfterDisabled = 1;
            mailMoveDelete(e.item.id);
            break;
        case "read":
            mailMarkAsReadUnread("read");
            break;
        case "unread":
            mailMarkAsReadUnread("unread");
            break;
        case "NewFolder":
            window.location.href = "../File/MailBoxFolders";
            break;
        case "print":
            mailPrint();
            break;
    }
}

function mailReply(id) {
    var grid = $("#mainWidget").data("kendoGrid");
    var selected = grid.dataItem(grid.select());

    $(".main-section").removeClass("horizontal").removeClass("vertical");

    if (!selected) {
        $(".main-section").load(baseUrl + '/Home/NewMail?id=' + id);
    }
    else {
        var subject = selected.Subject.replace(/ /g, '%20');
        var mailTo = selected.Email;
        $(".main-section").load(baseUrl + '/Home/NewMail?id=' + id + '&mailTo=' + mailTo + '&subject=' + subject);
    }
}

function mailForward(id) {
    var grid = $("#mainWidget").data("kendoGrid");
    var selected = grid.dataItem(grid.select());

    $(".main-section").removeClass("horizontal").removeClass("vertical");

    if (!selected) {
        $(".main-section").load(baseUrl + '/Home/NewMail?id=' + id);
    }
    else {
        var subject = selected.Subject.replace(/ /g, '%20');
        $(".main-section").load(baseUrl + '/Home/NewMail?id=' + id + '&subject=' + subject);
    }
}

function mailMoveDelete(id) {
    var grid = $("#mainWidget").data("kendoGrid");
    SelectedRowsForMove = grid.select().length;
    if (id == "Deleted") {
        kendo.confirm("Are you sure that you want to Delete " + SelectedRowsForMove + " record").then(function () {
            for (var i = 0; i < grid.select().length; i++) {
                var selectedItem = grid.dataItem(grid.select()[i]);
                selectedItem.FolderId = id;
                selectedItem.dirty = true;
            }
            grid.dataSource.sync();
        }, function () {
            return false;
        });
    }
    else {
        for (var i = 0; i < grid.select().length; i++) {
            var selectedItem = grid.dataItem(grid.select()[i]);
            selectedItem.FolderId = id;
            selectedItem.dirty = true;
        }
        grid.dataSource.sync();
    }


    //$("#mainWidget").data("kendoGrid").dataSource.read();

}

function mailMarkAsReadUnread(id) {
    var grid = $("#mainWidget").data("kendoGrid");
    var selectedRows = grid.select();
    var read;

    if (id === "read") {
        read = true;
    } else {
        read = false;
    }

    for (var i = 0; i < selectedRows.length; i++) {
        var item = grid.dataItem(selectedRows[i]);
        item.IsRead = read;
        item.dirty = true;
        read === true ? $(selectedRows[i]).removeClass("unread") : $(selectedRows[i]).addClass("unread");;
    }

    if (!read) {
        markedAsUnread = true;
    }

    grid.dataSource.sync();
}

function mailPrint() {
    var grid = $("#mainWidget").data("kendoGrid");
    if (grid.select().length != 0) {
        kendo.drawing.drawDOM($(".mail-details"))
        .then(function (group) {
            return kendo.drawing.exportPDF(group, {
                paperSize: "auto",
                margin: { left: "1cm", top: "1cm", right: "1cm", bottom: "1cm" }
            });
        })
        .done(function (data) {
            kendo.saveAs({
                dataURI: data,
                fileName: "Mail.pdf"
            });
        });
    }
}

// Switch between vertical / horizontal panes view
function changeToVerticalPanes(e) {
    var grid = $("#mainWidget").data("kendoGrid");
    grid.hideColumn(1);
    grid.hideColumn(2);
    grid.showColumn(3);
    updateSelectedClasses(e);
    $('.main-section').addClass("vertical");
    $('.main-section').removeClass("horizontal");
    grid.refresh();
}

function changeToHorizontalPanes(e) {
    var grid = $("#mainWidget").data("kendoGrid");
    grid.hideColumn(3);
    grid.showColumn(1);
    grid.showColumn(2);
    updateSelectedClasses(e);
    $('.main-section').addClass("horizontal");
    $('.main-section').removeClass("vertical");
    grid.refresh();
}

function updateSelectedClasses(element) {
    var selectedClass = "selected";

    $(".toolbar ." + selectedClass).removeClass(selectedClass);
    $(element).addClass(selectedClass);
}

// Select category handler in the sidebar navigation
function selectCategory(e) {
    var dataItem = this.dataItem(e.node);
    var selectedText = e.sender.dataItem(e.node).value;

    setMenuItemsAvailability(false, "noselection");
    $(".search-textbox").val('');
    $('input.master-checkbox').prop('checked', false);

    Cookies.set('selected', kendo.stringify(dataItem.index));
    Cookies.set('selectedNodeText', selectedText);

    selected = Cookies.get('selected');
    selectedNodeText = Cookies.get('selectedNodeText');

    navigated = true;
    Cookies.remove('ChangedIdOfFolder');
    filterGrid(selectedText);
}

// Filter grid according to the currently selected mails category
function filterGrid(selectedText) {
    var mailsGrid = $("#mainWidget").data("kendoGrid");
    if (!mailsGrid) {
        window.location.href = baseUrl + '/Home/Index';
    } else {
        mailsGrid.dataSource.filter({ field: "FolderId", operator: "equals", value: selectedText });
    }
    $("#mainWidget").find('tbody').css("display", "");
}

// Get the number of mails in each category
function getinitialNumberOfItems(gridData, MailBoxFolders) {
    var ActiveCount = 0;
    var DisabledCount = 0;
    var numbers = { Inbox: { TotalCount: 0, ActiveCount: 0, DisabledCount: 0 } };
    for (var FolderCount = 0; FolderCount < MailBoxFolders.length; FolderCount++) {
        var FolderName = MailBoxFolders[FolderCount].value;
        numbers[FolderName] = { TotalCount: 0, ActiveCount: 0, DisabledCount: 0 };
    }
    for (var FolderCountForActive = 0; FolderCountForActive < MailBoxFolders.length; FolderCountForActive++) {

        var FolderWiseTotalCount = 0;
        for (var i = 0; i < gridData.length; i++) {

            var currentItemCategory = gridData[i].FolderId;
            try {
                if (FolderCountForActive == 0) {
                    numbers[currentItemCategory].TotalCount += 1;
                }
                if (gridData[i].StatusId == "1" && gridData[i].FolderId == MailBoxFolders[FolderCountForActive].value) {
                    numbers[currentItemCategory].ActiveCount += 1;
                }
                else if (gridData[i].StatusId == "0" && gridData[i].FolderId == MailBoxFolders[FolderCountForActive].value) {
                    numbers[currentItemCategory].DisabledCount += 1;
                }
            }
            catch (err) {

            }

        }
    }

    return numbers;
}

$("body").on("click", "#BtnCloseNewEmail", function () {
    $("#PartalViewContet").css("display", "none");
    $("#mainViewContent").css("display", "");
});
$("#navigationTreeView").click(function () {
    if ($("#IsParialViewLoaded").val() == "1") {
        $("#PartalViewContet").css("display", "none");
        $("#mainViewContent").css("display", "");
        $("#IsParialViewLoaded").val("0");
    }
});

function dataSourceChange(e) {

    var grid = $("#mainWidget").data("kendoGrid");
    var treeview = $("#navigationTreeView").data("kendoTreeView");
    if (e.action === "sync") {
        var dataItem = treeview.dataItem(treeview.select());

        var data = grid.dataSource.data();
        var dataLength = data.length;

        for (var i = 0; i < dataLength; i++) {
            var currntItem = data[i];
            if (currntItem.FolderId !== dataItem.value) {
                i -= 1;
                dataLength -= 1;
                grid.dataSource.pushDestroy(currntItem);
            }
        }
    }

}

function dataSourceRequestEnd(e) {


    if (e.type == "update") {

        debugger;
        if (SelectedRowsForMove != "") {
            TotalSelectedRecordForMove = parseInt(TotalSelectedRecordForMove) + 1;
            //SelectedRowsForMove = "";
            if (SelectedRowsForMove == TotalSelectedRecordForMove) {
                SelectedRowsForMove = "";
                TotalSelectedRecordForMove = 0;
                $("#mainWidget").data("kendoGrid").dataSource.read();
            }

        }


    }
    else {
        setTimeout(function () {
            var grid = $("#mainWidget").data("kendoGrid");
            if (grid.dataSource.view().length == 0) {
                setMenuItemsAvailability(false, "noselection");
            }
        }, 100)
        if (AfterDisabled == 1) {
            // $("#mainWidget").data("kendoGrid").dataSource.read();
            var treeview = $("#navigationTreeView").data("kendoTreeView");
            var dataItem = treeview.dataItem(treeview.select());
            filterGrid(dataItem.value);
            AfterDisabled = 0;
        }
    }
}

var GridDataInAjaxCall;

function GetFoldersByMailBoxId(gridData, FromSearchFolder) {
    $.ajax({
        url: APIBaseUrl + 'api/ApiHome/GetMailBoxFolderList',
        cache: false,
        async: false,
        data: { MailBoxId: $("#ListOfMailBox").val(), SerchedFolderString: $("#txtFolderSearch").val(), UserId: $("#txtLoginUserId").val() },
        success: function (MailBoxFolders) {
            $('#MoveMenu').find('.k-menu-group').html('');
            var numbers = getinitialNumberOfItems(gridData.Data, MailBoxFolders);
            for (var FolderCount = 0; FolderCount < MailBoxFolders.length; FolderCount++) {
                var FolderName = MailBoxFolders[FolderCount].value;
                MailBoxFolders[FolderCount].number = numbers[FolderName].TotalCount;
                MailBoxFolders[FolderCount].Active = numbers[FolderName].ActiveCount;
                MailBoxFolders[FolderCount].Disable = numbers[FolderName].DisabledCount;
                var FolderId = MailBoxFolders[FolderCount].value;

                $(".disabledMenu").remove();
                var newds = '<li class="k-item k-state-default" id="' + FolderId + '" operation="moveDelete" role="menuitem" aria-disabled="false"><span class="k-link">' + MailBoxFolders[FolderCount].text + '</span></li>';
                $('#MoveMenu').find('.k-menu-group').append(newds);

                $('#Disable').css("display", "");
            }

            populateNavigationTree(MailBoxFolders, FromSearchFolder);

        },
        error: function (error) {
            //  alert('Error');
        }
    })
}

function getFolderList(FromSearchFolder) {

    $.ajax({
        url: APIBaseUrl + "api/ApiHome/GetEmailList?AjaxRequest=YES&MailBoxId=" + $("#ListOfMailBox").val() + "&UserId=" + $("#txtLoginUserId").val(),
        async: false,
        success: function (gridData) {
            GridDataInAjaxCall = gridData;
            GetFoldersByMailBoxId(gridData, FromSearchFolder);
        }

    });

}


// Attach mails grid events
function mailGridDataBound(e) {

    var grid = e.sender;

    for (var i = 0; i < grid.tbody.find(">tr").length; i++) {
        var item = grid.dataItem(grid.tbody.find(">tr")[i]);
        if (item.IsRead == false) {
            $(grid.tbody.find(">tr")[i]).addClass("unread")
        }
    }


    bindCheckboxes();
    polulateSelectedRows(grid);

    getFolderList();

    if (grid.select().length === 1) {
        $(".mail-details-wrapper").removeClass("empty");
    }
    else {
        $(".mail-details-wrapper").addClass("empty");
    }

    if (grid.dataSource.view().length === 0) {
        $('input.master-checkbox').prop('checked', false);
    };
    $("#mainWidget").removeClass("HideElement");

    $(function () {
        $(".notifications-switch").kendoMobileSwitch({
            onLabel: "Active",
            offLabel: "Disabled"
        });
    });

}


function mailSelectionChanged(e) {
    debugger;
    var selectedRows = this.select();

    selectionChanged(e.sender, 'mailsSelectedRow');
    checkSelectedCheckbox(selectedRows);

    if (selectedRows.length === 1) {
        var dataItem = this.dataItem(selectedRows[0]);
        populateDetailsView(dataItem);
        $(".mail-details-wrapper").removeClass("empty");

        setMenuItemsAvailability(true);
    } else {
        $(".mail-details-wrapper").addClass("empty");
        setMenuItemsAvailability(false, "multiselection");
    }
}

function selectionChanged(widget, selectedRowPrefix) {

}

// Populated selected rows, based on previously saved selection
function polulateSelectedRows(widget) {
    var navigationTreeView = $('#navigationTreeView').data('kendoTreeView');
    var treeViewSelectedItem = navigationTreeView.select();
    var widgetDataSource = widget.dataSource;

    if (treeViewSelectedItem.length === 1) {
        var treeViewDataItem = navigationTreeView.dataItem(treeViewSelectedItem);
        var treeViewItemValue = treeViewDataItem.value;
        var selectedRowsFromCoockie = Cookies.get('mailsSelectedRow' + treeViewItemValue);

        if (selectedRowsFromCoockie) {
            var selectedRowsArray = selectedRowsFromCoockie.split(',');

            for (var i = 0; i < selectedRowsArray.length; i++) {
                currentRowId = selectedRowsArray[i];
                var dataItem = widgetDataSource.get(currentRowId);
                if (dataItem) {
                    var row = widget.tbody.find("tr[data-uid='" + dataItem.uid + "']");
                    widget.select(row);

                    if (!marked && navigated) {
                        widget.content.scrollTop(row.offset().top - widget.content.offset().top);
                    } else if (marked && navigated) {
                        marked = false;
                        widget.content.scrollTop(savedScroll);
                    }

                    navigated = false;
                }
            }
        }
    }
}

// Bind mail ceckboxes
function bindCheckboxes() {
    $('.chkbx').on('change', function (e) {
        debugger;
        var target = $(e.target);
        var checked = e.target.checked;
        var mailsGrid = $("#mainWidget").data("kendoGrid");
        var selectedRows = mailsGrid.select();
        var checkedRow = $(target).parents('tr');

        if (checked) {
            checkedRow.addClass('k-state-selected');
            selectedRows = mailsGrid.select();
            mailsGrid.select(selectedRows);
        } else {
            $('.master-checkbox').prop('checked', false);

            var resultSelection = $.map(selectedRows, function (row) {
                if ($(row).attr('data-uid') !== checkedRow.attr('data-uid')) {
                    return row;
                }
            });

            checkedRow.removeClass('k-state-selected');
            mailsGrid.select(resultSelection);

            if (resultSelection.length === 0) {
                $('input.master-checkbox').prop('checked', false);
                $(".mail-details-wrapper").addClass("empty");
                setMenuItemsAvailability(false, "noselection");
            }
        }
    });
}

// Configure menu items availability
function setMenuItemsAvailability(isEnabled, selection) {

    //var menu = $('#mailMenu').data('kendoMenu');
    //var contextMenu = $('#mailContextMenu').data('kendoContextMenu');

    //if (isEnabled) {
    //    toggleEnableMenuItems(menu, "mailMenu", isEnabled);
    //    toggleEnableMenuItems(contextMenu, "mailContextMenu", isEnabled);
    //} else if (!isEnabled && selection == "noselection") {
    //    toggleEnableMenuItems(menu, "mailMenu", isEnabled);
    //} else if (!isEnabled && selection == "multiselection") {
    //    var itemsIds = ["RE", "RE_ALL", "FW", "print"];

    //    toggleEnableMenuItems(menu, "mailMenu", true);

    //    itemsIds.forEach(function (itemID) {          
    //        $("#mailMenu").find(".k-item[id=" + itemID + "]").each(function (index) {
    //            menu.enable($(this), isEnabled);
    //        });
    //        $("#mailContextMenu").find(".k-item[id=" + itemID + "]").each(function (index) {
    //            contextMenu.enable($(this), isEnabled);
    //        });
    //    });
    //}
    //debugger;
    //if (OtherOwner == "YES")
    //{
    //    var menu = $("#mailMenu").kendoMenu().data("kendoMenu");
    //    menu.enable("#Disable", false);
    //}

}

function toggleEnableMenuItems(widget, widgetId, isEnabled) {
    $("#" + widgetId).find(".k-item").each(function (index) {
        widget.enable($(this), isEnabled);
    });
}

// Check checkbox on mail selection
function checkSelectedCheckbox(selectedRows) {
    debugger
    var mailsGrid = $('#mainWidget').data('kendoGrid');
    var mailsInView = mailsGrid.dataSource.view().length;

    $('input.chkbx').prop('checked', false);

    if (mailsInView > selectedRows.length) {
        $('input.master-checkbox').prop('checked', false);
    } else {
        $('input.master-checkbox').prop('checked', true);
    }
    var checkboxes = selectedRows.find('.chkbx');
    checkboxes.prop('checked', true);
}

// Populate the details view with the selected mail
function populateDetailsView(item) {
    $('.mail-subject').text(item.Subject);
    $('.mail-sender').text(item.To);
    $('.mail-date').text(item.Date);
    $('.mail-text').html(item.Text);
}