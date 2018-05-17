
$(document).ready(function () {
    EditUserDepartment.Init();
});

EditUserDepartment = {
    Init: function () {
        //ideally, we'd load


        //organization select change

        //department checked change - toggles display of groups
        $("html").on("change", ":checkbox[data-department-id]", function () {
            var deptId = $(this).attr('data-department-id');
            var isChecked = $(this).is(':checked');
            if (isChecked) $('div.department-group-container[data-department-id=' + deptId + ']').show();
            else $('div.department-group-container[data-department-id=' + deptId + ']').hide();
        });
        $(':checkbox[data-department-id]').on('change', function () {
            
        });

        $("#cmbOrganizations").on('change', function () {
            var orgId = $(this).val();
            console.log(orgId);
            $('div.organization-container[data-organization-id]').hide();
            $('div.organization-container[data-organization-id=' + orgId + ']').show(); 

        });
        $(".add-group-button").on('click', function () {
            var deptId = $(this).attr('data-department-id');
            //var orgId = $(this).closest('div.organization-container').attr('data-organization-id');
            $("div.add-group-container[data-department-id=" + deptId + "]").show();
            $(this).hide();

            $("div.save-group-buttons[data-department-id=" + deptId + "]").show();
        });

        $(".cancel-add-group-button").on('click', function () {
            var deptId = $(this).attr('data-department-id');
            $("div.add-group-container[data-department-id=" + deptId + "]").hide();
            $(".add-group-button[data-department-id=" + deptId + "]").show();
            $("div.save-group-buttons[data-department-id=" + deptId + "]").hide();
        });
        $(".save-group-button").on('click', function () {
            var deptId = $(this).attr('data-department-id');
            var groupName = $("div.add-group-container[data-department-id=" + deptId + "]").find('input[type=text]').val();
            if (groupName === null || groupName === undefined || groupName === '') return false;
            var model = {
                DepartmentId: Number(deptId),
                GroupName: groupName
            };

            $.ajax({
                url: "/Account/AddOrganizationDepartment",
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(model),
                async: true,
                cache: false,
                dataType: "html",
                success: function success(results) {
                    console.log(results);
                    console.log($(".organization-container[data-organization-id=" + orgId + "] div.organization-group-container").length);
                    $(".organization-container[data-organization-id=" + orgId + "] div.organization-group-container").append(results);
                    $("div.add-department-container[data-organization-id=" + orgId + "]").hide();
                    $(".add-department-button[data-organization-id=" + orgId + "]").show();
                    $("div.save-department-buttons[data-organization-id=" + orgId + "]").hide();
                    //MessageDialog("New Department Added");
                },
                error: function error(err) {
                    alert('error');
                }
            });



        });

        $(".add-department-button").on('click', function () {
            //var orgId = $(this).attr('data-organization-id');
            var orgId = $(this).closest('div.organization-container').attr('data-organization-id');
            $("div.add-department-container[data-organization-id=" + orgId + "]").show();
            $(this).hide();

            $("div.save-department-buttons[data-organization-id=" + orgId + "]").show();
        });

        $(".save-department-button").on('click', function () {
            var orgId = $(this).attr('data-organization-id');
            var deptName = $("div.add-department-container[data-organization-id=" + orgId + "]").find('input[type=text]').val();
            if (deptName === null || deptName === undefined || deptName === '') return false;
            var model = {
                OrganizationId: Number(orgId),
                DepartmentName: deptName
            };

            $.ajax({
                url: "/Account/AddOrganizationDepartment",
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(model),
                async: true,
                cache: false,
                dataType: "html",
                success: function success(results) {
                    console.log(results);
                    console.log($(".organization-container[data-organization-id=" + orgId + "] div.organization-group-container").length);
                    $(".organization-container[data-organization-id=" + orgId + "] div.organization-group-container").append(results);
                    $("div.add-department-container[data-organization-id=" + orgId + "]").hide();
                    $(".add-department-button[data-organization-id=" + orgId + "]").show();
                    $("div.save-department-buttons[data-organization-id=" + orgId + "]").hide();
                    //MessageDialog("New Department Added");
                },
                error: function error(err) {
                    alert('error');
                }
            });



        });
        $(".delete-department-button").on('click', function () {
            var deptId = $(this).attr('data-department-id');
            var name = $(this).attr('data-department-name');
            Confirm("Do you want to delete the department " + name + "?",
                function () {
                    var obj = { DepartmentId: deptId };
                    $.ajax({
                        url: "/Account/DeleteDepartment",
                        type: 'POST',
                        contentType: 'application/json',
                        data: JSON.stringify(obj),
                        async: true,
                        cache: false,
                        success: function success(results) {
                            var id = results.id;
                            $('div.department-container[data-department-id=' + id + ']').remove();

                            //MessageDialog("New Department Added");
                        },
                        error: function error(err) {
                            alert('error');
                        }
                    });
                },
                function () {
                    //do nothing
                });
        });


        $(".delete-group-button").on('click', function () {
            var groupId = $(this).attr('data-group-id');
            var name = $(this).attr('data-group-name');
            Confirm("Do you want to delete the group " + name + "?",
                function () {
                    var obj = { GroupId: groupId };
                    $.ajax({
                        url: "/Account/DeleteDepartmentGroup",
                        type: 'POST',
                        contentType: 'application/json',
                        data: JSON.stringify(obj),
                        async: true,
                        cache: false,
                        success: function success(results) {
                            var id = results.id;
                            $('div.group-container[data-group-id=' + id + ']').remove();

                            //MessageDialog("New Department Added");
                        },
                        error: function error(err) {
                            alert('error');
                        }
                    });
                },
                function () {
                    //do nothing
                });
        });
        $(".cancel-add-department-button").on('click', function () {
            var orgId = $(this).attr('data-organization-id');
            $("div.add-department-container[data-organization-id=" + orgId + "]").hide();          
            $(".add-department-button[data-organization-id=" + orgId + "]").show();
            $("div.save-department-buttons[data-organization-id=" + orgId + "]").hide();
        });

        $("#btnSave").on('click', function () {

            //array of number (department id)
            //we cant just send groips b/c you can be part of a department without being in a group within the department
            var submitObj = [];
            var orgId = $("#cmbOrganizations").val();
           
            $('div.organization-container[data-organization-id=' + orgId + ']').each(function () {
                //loop through each checkbox with 
                $(this).find(':checkbox[data-department-id]').each(function () {
                    if ($(this).is(':checked')) {
                        var deptId = $(this).attr('data-department-id');
                        var deptObj = { DepartmentId: deptId, GroupIds: [] };
                        $('div.department-group-container[data-department-id=' + deptId +'] :checkbox[data-group-id]').each(function () {
                            if ($(this).is(':checked')) {
                                deptObj.GroupIds.push($(this).attr('data-group-id'));
                            }
                        });
                        submitObj.push(deptObj);
                    }

                });
                //var $deptChk = $(this).find(':checkbox[data-department-id]');
                


            });
            //console.log(submitObj);
            $.ajax({
                url: "/Account/SaveUserDepartments",
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(submitObj),
                async: true,
                cache: false,
                //dataType: "html",
                success: function success(results) {
                    console.log('save department success');
                    MessageDialog("Your group subscription updates have been saved");

                },
                error: function error(err) {
                    alert('error');
                }
            });

            console.log(submitObj);


        });

    }

}