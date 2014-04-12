/*
* jDownload - A jQuery plugin to assist file downloads
* Examples and documentation at: http://jdownloadplugin.com
* Version: 1.3 (18/11/2010)
* Copyright (c) 2010 Adam Chambers, Tim Myers
* Licensed under the GNU General Public License v3: http://www.gnu.org/licenses/gpl.html
* Requires: jQuery v1.4+ & jQueryUI 1.8+
*/

(function($) {

    $.fn.jDownload = function(settings) {

        var config = {
            root: "../",
            filePath: null,
            event: "click", // default click event??
            dialogTitle: "文件下载",
            dialogDesc: '马上下载文件?',
            dialogWidth: 450,
            dialogHeight: 'auto',
            dialogModal: true,
            showfileInfo: true,
            start: null,
            stop: null,
            download: null,
            cancel: null
        }

        settings = $.extend(config, settings);

        var dialogID = "jDownloadDialog_" + $('.jDownloadDialog').length;
        var iframeID = "jDownloadFrame_" + $('.jDownloadFrame').length;

        // create html iframe and dialog
        var iframeHTML = '<iframe class="jDownloadFrame" src="" id="' + iframeID + '"></iframe>';
        var dialogHTML = '<div class="jDownloadDialog" title="' + settings.dialogTitle + '" id="' + dialogID + '"></div>';

        // append both to document
        $('body').append(iframeHTML + dialogHTML);


        var iframe = $('#' + iframeID);
        var dialog = $('#' + dialogID);

        // set iframe styles
        iframe.css({
            "height": "0px",
            "width": "0px",
            "visibility": "hidden"
        });

        // set dialog options
        dialog.dialog({
            autoOpen: false,
            buttons: {
                "取消": function() {
                    if ($.isFunction(settings.cancel)) {
                        settings.cancel();
                    }
                    $(this).dialog('close');
                },

                "下载": function() {
                    if ($.isFunction(settings.download)) {
                        settings.download();
                    }
                    start_download();
                }
            },
            width: settings.dialogWidth,
            height: settings.dialogHeight,
            modal: settings.dialogModal,
            close: ($.isFunction(settings.stop)) ? settings.stop : null
        });


        $(this).bind(settings.event, function() {

            if ($.isFunction(settings.start)) {
                settings.start();
            }

            var _this = $(this);


            dialog.html("");

            // if filePath is not specified then use the href attribute
            var filePath = (settings.filePath == null) ? $(this).attr('href') : settings.filePath;

            dialog.html('<p>正在获取文件信息...</p><img src="' + settings.root + 'loader.gif" alt="获取信息中" />');

            $.ajax({
                type: "POST",
                url: settings.root + "jdownload.aspx?action=info&path=" + encodeURIComponent(filePath),
                dataType: "json",
                error: function(XMLHttpRequest, textStatus, errorThrown) {
                    dialog.html("<p class=\"jDownloadError\">获取文件时出现错误</p>");
                },
                success: function(data) {
                    setTimeout(function() {
                        if (data.Error == "error") {
                            dialog.html("<p class=\"jDownloadError\">无法找到文件</p>");
                        } else {

                            if (settings.showfileInfo == true) {

                                // parse JSON
                                var html = "<div class=\"jDownloadInfo\">";
                                html += "<p><span>文件名称:</span> " + data.FileName + "</p>";
                                html += "<p><span>文件类型:</span> " + data.FileType + "</p>";
                                html += "<p><span>文件大小:</span> " + data.FileSize + " KB</p>";
                                html += "</div>";

                                // remove any old file info & error messages
                                $('.jDownloadInfo, .jDownloadError').remove();

                                var desc = (_this.attr('title').length > 0) ? _this.attr('title') : settings.dialogDesc;

                                // append new file info
                                dialog.html('<p>' + desc + '</p>' + html);
                            }
                        }
                    }, 200);
                }
            });

            // open dialog 
            dialog.data('jDownloadData', { filePath: filePath }).dialog('open');

            return false;

        });

        /* Iniate download when value Ok is iniated via the dialog */
        function start_download(i) {


            // change iframe src to fieDownload.php with filePath as query string??
            iframe.attr('src', settings.root + 'jdownload.aspx?action=download&path=' + encodeURIComponent(dialog.data('jDownloadData').filePath));

            // Close dialog
            dialog.dialog('close');

            return false;
        }

    }

})(jQuery);