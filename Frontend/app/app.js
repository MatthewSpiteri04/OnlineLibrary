var OnlineLibrary = angular.module('OnlineLibrary', []);

// USERS CONTROLLER
OnlineLibrary.controller('users-controller', ['$scope', '$http', function($scope, $http){
    $scope.currentUser = {};

    $scope.doesUserExist = function(loginForm) {
        $scope.userFound = false;

        $http.get('https://localhost:44311/api/User/Exists/'+ loginForm.username)
            .then(data => {
                $scope.login(data, loginForm);
            })
    };

    $scope.login = function(userFound, loginForm) {
        if (userFound) {
            $http.get('https://localhost:44311/api/User/Login/' + loginForm.username + '/' + loginForm.password)
            .then(data => {
                $scope.currentUser = data;
                console.log($scope.currentUser)
            })
        };
    };
}]);