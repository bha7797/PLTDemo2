// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// ----------------------------------------------------------------------------

function initializeEventHandlers() {
    // Workspace select event
    globals.workspaceSelect.on("change", function () {
        let getSelectParams = {
            workspaceId: this.value,
            workspaceName: $('#workspace-select option:selected').text()
        };
        getReports(getSelectParams);
    });

    // If User click on Save button in Add Reports section
    globals.reports_table.on("click", "td:last-child", function () {
        var self = $(this).children("img");
        //Show spinner in place of Save icon till data gets stored
        this.children[0].src = '../Content/img/Load.svg';
        //If click on Page's Save button
        if (this.id != '') {
            let arr = this.id.split('/');
            let embedParam = {
                workspaceId: arr[0],
                reportId: arr[1],
                pageId: arr[2],
                workspaceName: arr[3],
                reportName: arr[4],
                pageName: arr[5]
            }
            $.ajax({
                url: "/Report/Reportids",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(embedParam),
                success: function (response) {
                    $('.postSaveMessageText').html(response.responseText);
                    $('.postSaveMessage').show();
                    self.attr('src', '../Content/img/Save.svg');
                    setTimeout(() => {
                        $('.postSaveMessage').hide(200);
                    }, "5000");
                },
                error: function (err) {
                    $('.postSaveMessageText').html(err);
                    $('.postSaveMessage').show();
                    self.attr('src', '../Content/img/Save.svg');
                }
            });
        } else { //If click on Report's Save button
            let arr = this.previousElementSibling.previousElementSibling.id.split('/');
            let getSelectParams = {
                workspaceId: arr[0],
                reportId: arr[1],
                workspaceName: arr[2],
                reportName: arr[3],
                saveInDb: true,
                el: this
            }
            getReportPages(getSelectParams);
        }
    })

    // If User click on Expand/Compress button in Add Reports section
    globals.reports_table.on("click", "td:first-child", function () {
        let arr = this.id.split('/');
        if (this.children[0].className == 'expanded') {
            this.children[0].className = 'compressed'
            this.children[0].src = '../Content/img/Arrow 1.svg';
            $('.pages').hide();
            return;
        }
        var el = globals.reports_table.children(0).children(0).children(0);
            for (let i = 0; i < el.length; i++) {
                if (el[i].className == 'expanded') {
                    el[i].className = 'compressed'
                    el[i].src = '../Content/img/Arrow 1.svg';
                }
            }
            //Hide all the opened pages first  
        $('.pages').hide();

        this.children[0].className = 'expanded'
        this.children[0].src = '../Content/img/Arrow 2.svg';

        //Check if report pages are already loaded, if yes then show instead of fetching it from DB again
        var siblingTrNode = this.parentNode.nextSibling;
        if (siblingTrNode && siblingTrNode.className == 'pages') {
            while (siblingTrNode.className == 'pages') {
                siblingTrNode.style.display = 'revert';
                siblingTrNode = siblingTrNode.nextSibling;
            }
            return false;
        }
        //Else fetch from DB and show
        let getSelectParams = {
            workspaceId: arr[0],
            reportId: arr[1],
            workspaceName: arr[2],
            reportName: arr[3],
            saveInDb: false,
            el: this
        }
        getReportPages(getSelectParams);        
    });

    //Triggered when selecting from Workspace dropdown to filter PLTs
    globals.pltWorkspaceSelect.on("change", function () {
        //globals.reportSelect.removeAttr('disabled');
        const reports = new Set();
        let selectedWorkspaceName = $('#plt_workspace-select option:selected').val();
        $('.reportName_table tr').each(function () {
            if ($(this).attr('workspace-name') === selectedWorkspaceName || selectedWorkspaceName === '' || $(this).attr('workspace-name') == undefined) {
                $(this).show();
                reports.add($(this).attr('report-name'));
            }
            else {
                $(this).hide();
            }
        })
        $('#report-select option').each(function () {
            if (reports.has(this.text) || this.value === '') {
                $(this).show();
            }
            else {
                $(this).hide();
            }
        })
        $('#report-select').val('');
    });

    //Triggered when selecting from Report dropdown to filter PLTs
    globals.reportSelect.on("change", function () {
        let selectedWorkspaceName = $('#plt_workspace-select option:selected').val();
        let selectedReportName = $('#report-select option:selected').val();
        $('.reportName_table tr').each(function () {
            if ($(this).attr('workspace-name') === selectedWorkspaceName || selectedWorkspaceName === '') {
                if (
                    $(this).attr('report-name') === selectedReportName || selectedReportName === ''
                ) {
                    $(this).show();
                }
                else {
                    $(this).hide();
                }
            }

        });
    });

    $('#userName').on('click', () => {
        if ($('.user-profile').css('display') == 'none') $('.user-profile').show(500);
        else $('.user-profile').hide(500);
    });

    $(document).on("click", (e) => {
        var container = $('.user-profile');
        if ($('.user-profile').css('display') == 'block') {
        // if the target of the click isn't the container nor a descendant of the container
        if (!container.is(e.target) && container.has(e.target).length === 0 && !$('#userName').is(e.target)) {
                container.hide(500);
            }
        }
    });

    $('.signout').on("click", () => {
        $.ajax({
            url: "/Token/Logout",
            type: "POST",
            success: function (response) {
                if (window.location.pathname == '/') $('.plt_addreport').click();
                else location.reload(true);
            },
            error: function (err) {
                console.log("err: ", err.responseText);
            }
        });
    });

    $('.embed_close-svg').on("click", () => {
        $('.postSaveMessage').hide();
    })

    function refreshAccessToken() {
        $.ajax({
            url: "/Token/Index",
            type: "POST",
        });
    };

    setInterval(() => {
        /*var accessToken = loggedInUser.accessToken;
        var expiryTime = JSON.parse(atob(accessToken.split(".")[1])).exp * 1000;
        var timeLeftToExpire = (expiryTime - Date.now()) / 60000;
        console.log(accessToken, timeLeftToExpire);
        if (timeLeftToExpire < 5)*/
        refreshAccessToken();
    }, 3600000);
}