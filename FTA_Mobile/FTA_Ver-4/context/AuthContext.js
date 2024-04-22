import React, { createContext, useState, useEffect } from "react";
import { Alert } from "react-native";
import * as Location from "expo-location";
import {
  GoogleSignin,
  statusCodes,
} from "@react-native-google-signin/google-signin";
import AsyncStorage from "@react-native-async-storage/async-storage";

import createAxios from "../utils/AxiosUtility";

const API = createAxios();

export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [isLoading, setIsLoading] = useState(false);
  const [userInfo, setUserInfo] = useState(null);
  const [authState, setAuthState] = useState({
    token: "",
    authenticated: false,
  });

  GoogleSignin.configure({
    // webClientId: '<FROM DEVELOPER CONSOLE>', // client ID of type WEB for your server. Required to get the idToken on the user object, and for offline access.
    iosClientId:
      "611874810536-ea5432vg9er0nb16i4drj14tv5rv6i8v.apps.googleusercontent.com", // [iOS] if you want to specify the client ID of type iOS (otherwise, it is taken from GoogleService-Info.plist)
  });

  const register = async (user) => {
    setIsLoading(true);
    const response = await API.post("/auth/register", user);
    response &&
      Alert.alert(
        "Đăng ký thành công",
        "Chúc mừng quý khách vừa tạo tài khoản thành công, quý khách đã có thể đăng nhập vào hệ thống",
        [{ text: "OK" }],
      );
    setIsLoading(false);
  };

  const login = async (email, password) => {
    setIsLoading(true);
    const response = await API.post("/auth/login", {
      email: email,
      password: password,
    });

    if (response && response.response) {
      if (
        response.response.status === 400 ||
        response.response.status === 401
      ) {
        Alert.alert(
          "Sai tên tài khoản hoặc mật khẩu",
          "Vui lòng đăng nhập lại",
          [{ text: "OK" }],
        );
      }
    } else {
      setAuthState({
        token: response.token,
        authenticated: true,
      });

      await AsyncStorage.setItem(
        "TOKEN_KEY",
        JSON.stringify({ token: response.token, loggedIn: "systemLog" }),
      );

      const userInfoRes = await getProfile(response.token);
      const qtyInCartRes = await getCartQuantity(response.token);

      if (userInfoRes && qtyInCartRes !== null) {
        const userLocationRes = await getLocation(response.token);

        if (
          userLocationRes &&
          userLocationRes.statusCode !== 404 &&
          userLocationRes.statusCode <= 500
        ) {
          const location = userLocationRes.payload.find(
            (item) => item.isDefault,
          );
          setUserInfo({
            info: userInfoRes,
            qtyInCart: qtyInCartRes,
            location: location,
          });
          await AsyncStorage.setItem(
            "userInfo",
            JSON.stringify({
              info: userInfoRes,
              qtyInCart: qtyInCartRes,
              location: location,
            }),
          );
        } else {
          setUserInfo({
            info: userInfoRes,
            qtyInCart: qtyInCartRes,
          });
          await AsyncStorage.setItem(
            "userInfo",
            JSON.stringify({
              info: userInfoRes,
              qtyInCart: qtyInCartRes,
            }),
          );
        }
      }
    }
    setIsLoading(false);
  };

  const onBtnGoogleLoginHandler = async () => {
    setIsLoading(true);
    try {
      await GoogleSignin.hasPlayServices({
        showPlayServicesUpdateDialog: true,
      });
      const userInfo = await GoogleSignin.signIn();

      setUserInfo(userInfo.user);
      setAuthState({
        token: userInfo.idToken,
        authenticated: true,
      });
      await AsyncStorage.setItem(
        "TOKEN_KEY",
        JSON.stringify({ token: userInfo.idToken, loggedIn: "google" }),
      );
      await AsyncStorage.setItem("userInfo", JSON.stringify(userInfo.user));
      // switch(account.role) {
      //   case "unknown":
      //     const signUp_Response = API.post("/signup", {
      //       role: "customer",
      //       address: loggedUser?.address || "",
      //       user: {
      //         photo: userInfo.user.photo,
      //         name: userInfo.user.name,
      //         phone: loggedUser?.phoneNumber || "",
      //         email: userInfo.user.email,
      //       },
      //     });
      //     await navigation.navigate("Profile");
      //     break;
      //   case "customer":
      //     break;
      // }
    } catch (error) {
      if (error.code === statusCodes.SIGN_IN_CANCELLED) {
        // user cancelled the login flow
      } else if (error.code === statusCodes.IN_PROGRESS) {
        // operation (e.g. sign in) is in progress already
      } else if (error.code === statusCodes.PLAY_SERVICES_NOT_AVAILABLE) {
        // play services not available or outdated
      } else {
        // some other error happened
      }
    }
    setIsLoading(false);
  };

  const logout = () => {
    setIsLoading(true);
    setAuthState(null);
    setUserInfo(null);
    AsyncStorage.removeItem("TOKEN_KEY");
    AsyncStorage.removeItem("userInfo");
    setIsLoading(false);
  };

  const isLoggedIn = async () => {
    setIsLoading(true);
    try {
      const tokenStorage = await AsyncStorage.getItem("TOKEN_KEY");

      if (tokenStorage !== null) {
        const data = JSON.parse(tokenStorage);
        const userInfo = await AsyncStorage.getItem("userInfo");
        const userInfoJsonParse = JSON.parse(userInfo);

        console.log("Token storage: " + JSON.stringify(data, null, 2));
        console.log("User info: " + JSON.stringify(userInfoJsonParse, null, 2));

        setAuthState({
          token: data.token,
          authenticated: true,
        });
        setUserInfo(userInfoJsonParse);
        updateCartQty(data.token);
      }
    } catch (e) {
      console.log(`Error occurred at: isLoggedIn error: ${e}`);
    }
    setIsLoading(false);
  };

  const getCartQuantity = async (token) => {
    const response = await API.customRequest("get", "/carts", null, token);
    let qtyInCart = response.payload ? response.payload.length : 0;
    console.log("get cart qty: " + JSON.stringify(qtyInCart, null, 2));
    return qtyInCart;
  };

  const updateCartQty = async (token) => {
    const qtyRes = await getCartQuantity(token);
    const userInfo = await AsyncStorage.getItem("userInfo");

    if (qtyRes && userInfo !== null) {
      const userInfoJsonParse = JSON.parse(userInfo);

      userInfoJsonParse.qtyInCart = qtyRes;

      console.log(
        "Qty after updating: " + JSON.stringify(userInfoJsonParse, null, 2),
      );

      setUserInfo(userInfoJsonParse);
      await AsyncStorage.setItem("userInfo", JSON.stringify(userInfoJsonParse));
    }
  };

  const getProfile = async (token) => {
    const response = await API.customRequest("get", "/aboutMe", null, token);
    return response;
  };

  const updateProfile = async (user) => {
    const response = await API.customRequest(
      "put",
      "/update-profile",
      user,
      authState?.token,
    );

    if (response) {
      const getInfoRes = await getProfile(authState?.token);
      const userInfo = await AsyncStorage.getItem("userInfo");

      if (getInfoRes && userInfo !== null) {
        const userInfoJsonParse = JSON.parse(userInfo);

        userInfoJsonParse.info = getInfoRes;

        console.log(
          "User info after updating: " +
            JSON.stringify(userInfoJsonParse, null, 2),
        );

        setUserInfo(userInfoJsonParse);
        await AsyncStorage.setItem(
          "userInfo",
          JSON.stringify(userInfoJsonParse),
        );
      }
    }
  };

  const getLocation = async (token) => {
    try {
      const { status } = await Location.requestForegroundPermissionsAsync();
      if (status !== "granted") {
        console.log("Permission to access location was denied");
      } else {
        const location = await Location.getCurrentPositionAsync();
        console.log(
          "Access location data: " + JSON.stringify(location, null, 2),
        );
        const response = await API.customRequest(
          "get",
          "/apartment-station",
          null,
          token,
        );
        console.log(
          "Location info: " + JSON.stringify(response.payload, null, 2),
        );
        return response;
      }
    } catch (error) {
      console.error("Error request location permission: ", error);
    }
  };

  // const updateLocation = async (apartmentData, stationData, isDefault) => {
  //   if (apartmentData && stationData) {
  //     const response = await API.customRequest(
  //       "/post",
  //       "/apartment-station/upsert",
  //       {
  //         stationId: stationData.id,
  //         apartmentId: apartmentData.id,
  //         isDefault: isDefault,
  //       },
  //       authState?.token,
  //     );
  //     console.log("Save location: " + JSON.stringify(response, null, 2));
  //   }
  // };

  useEffect(() => {
    if (authState?.authenticated) {
      updateCartQty(authState?.token);
      updateProfile(authState?.token);
    }
  }, [authState]);

  useEffect(() => {
    isLoggedIn();
  }, []);

  const value = {
    onBtnGoogleLoginHandler,
    register,
    login,
    logout,
    isLoading,
    authState,
    userInfo,
    updateProfile,
    updateCartQty,
    getLocation,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};
