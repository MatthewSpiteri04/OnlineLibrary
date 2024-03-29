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
        $http.get('https://localhost:44311/api/User/Exists/'+ loginForm.login)
            .then(response => {
                $scope.login(response.data, loginForm);
            })
    };

    $scope.login = function(userFound, loginForm) {
        if (userFound) {
            $http.post('https://localhost:44311/api/User/Login', {login: loginForm.login, password: loginForm.password})
            .then(response => {
                if (response.status == 200) {
                    $scope.currentUser = response.data;
                }
                else {
                    console.log('NOT FOUND');
                }
                
            });
        }
        else {
            console.log('NOT FOUND');
        };
    };

    $scope.signUpUser = function(signUpForm) {
        request = {
            firstName: signUpForm.firstName,
            lastName: signUpForm.lastName,
            email: signUpForm.email,
            username: signUpForm.username,
            password: signUpForm.password,
            passwordConfirmation: signUpForm.passwordConfirmation
        }
        console.log(request);

        if (request.password == request.passwordConfirmation) {
            $http.post('https://localhost:44311/api/User/Add', request)
            .then(response => {
                if (response.status == 200) {
                    $scope.currentUser = response.data;
                }
                else {
                    console.log('FAILED TO CREATE USER');
                }
            });
        }
        else {
            console.log("PASSWORDS DON'T MATCH");
        }

        
    };
}]);