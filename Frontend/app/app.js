var OnlineLibrary = angular.module('OnlineLibrary', ['ngRoute']);

OnlineLibrary.config(['$routeProvider', function($routeProvider) {
    $routeProvider
    .when('/home', {
        templateUrl:'views/home.html'
    })
    .when('/login', {
        templateUrl:'views/login.html',
        controller: 'users-controller'
    })
    .when('/sign-up', {
        templateUrl:'views/sign-up.html',
        controller: 'users-controller'
    })
    .otherwise({
        redirectTo: '/home'
    });
}]);

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