var OnlineLibrary = angular.module('OnlineLibrary', []);

// USERS CONTROLLER
OnlineLibrary.controller('users-controller', ['$scope', function($scope){
    $scope.currentUser = {};

    $scope.login = function(loginForm) {
        $scope.userFound = false;
    
        fetch('https://localhost:44311/api/User/Exists/'+ loginForm.username)
        .then(response => {
            return response.json();
        })
        .then(data => {
            $scope.$apply(() => {
                $scope.loginUser(data, loginForm);
            });
        });
    };

    $scope.loginUser = function(userFound, loginForm) {
        if (userFound) {
            fetch('https://localhost:44311/api/User/Login/' + loginForm.username + '/' + loginForm.password)
            .then(response => {
                return response.json();
            })
            .then(data => {
                $scope.$apply(() => {
                    $scope.currentUser = data;
                });
            })
        };
    };
    

}]);