import React, { useContext } from "react";
import Ionicons from "@expo/vector-icons/Ionicons";
import { View } from "react-native";
import { createBottomTabNavigator } from "@react-navigation/bottom-tabs";
import { getFocusedRouteNameFromRoute } from "@react-navigation/native";

import HomeScreen from "../screens/HomeScreen";
import CategoriesScreen from "../screens/CategoriesScreen";
import TodayScreen from "../screens/TodayScreen";
import WalletScreen from "../screens/WalletScreen";
import ProfileScreen from "../screens/ProfileScreen";
import { Colors } from "../constants/colors";
import { AuthContext } from "../context/AuthContext";

const Tab = createBottomTabNavigator();

const getTabBarVisibility = (route) => {
  const routeName = getFocusedRouteNameFromRoute(route) ?? "Home";
  console.log(routeName);

  if (routeName === "ProductDetail") {
    return "none";
  }

  return "flex";
};

function TabNavigator() {
  const { authState } = useContext(AuthContext);

  return (
    <Tab.Navigator
      screenOptions={{
        headerShown: false,
        tabBarActiveTintColor: Colors.primaryGreen700,
        tabBarLabelStyle: {
          fontWeight: "500",
          fontSize: 12,
          marginBottom: 4,
        },
        tabBarStyle: {
          backgroundColor: "#fff",
          height: 90,
        },
      }}
    >
      <Tab.Screen
        name="HomeTab"
        component={HomeScreen}
        options={({ route }) => ({
          title: "Trang chủ",
          // tabBarStyle: {
          //   display: getTabBarVisibility(route),
          // },
          tabBarIcon: ({ focused, color, size }) => (
            <Ionicons
              name={focused ? "home" : "home-outline"}
              color={color}
              size={size}
            />
          ),
        })}
      />
      {authState?.authenticated ? (
        <>
          <Tab.Screen
            name="CategoryTab"
            component={CategoriesScreen}
            options={{
              title: "Danh mục",
              tabBarIcon: ({ focused, color, size }) => (
                <Ionicons
                  name={focused ? "layers" : "layers-outline"}
                  color={color}
                  size={size}
                />
              ),
            }}
          />
          <Tab.Screen
            name="TodayScreen"
            component={TodayScreen}
            options={{
              title: "Hôm nay",
              tabBarIcon: ({ size }) => (
                <View
                  style={{
                    backgroundColor: Colors.primaryGreen700,
                    borderRadius: 50,
                    padding: 12,
                    marginTop: -20,
                    shadowColor: "#7F5DF0",
                    shadowOffset: {
                      width: 0,
                      height: 4,
                    },
                    shadowOpacity: 0.25,
                  }}
                >
                  <Ionicons name="flash" color="#fff" size={size} />
                </View>
              ),
            }}
          />
          <Tab.Screen
            name="Wallet"
            component={WalletScreen}
            options={{
              title: "Ví",
              tabBarIcon: ({ focused, color, size }) => (
                <>
                  <Ionicons
                    name={focused ? "wallet" : "wallet-outline"}
                    color={color}
                    size={size}
                  />
                  {/*
                <Badge
                  style={{
                    position: "absolute",
                    top: 0,
                    right: 20,
                    backgroundColor: "#FF2929",
                  }}
                >
                  6
                </Badge>
                */}
                </>
              ),
            }}
          />
          <Tab.Screen
            name="Profile"
            component={ProfileScreen}
            options={{
              title: "Cá nhân",
              tabBarIcon: ({ focused, color, size }) => (
                <Ionicons
                  name={focused ? "person" : "person-outline"}
                  color={color}
                  size={size}
                />
              ),
            }}
          />
        </>
      ) : (
        <>
          <Tab.Screen
            name="TodayScreen"
            component={TodayScreen}
            options={{
              title: "Hôm nay",
              tabBarIcon: ({ focused, color, size }) => (
                <Ionicons
                  name={focused ? "flash" : "flash-outline"}
                  color={color}
                  size={size}
                />
              ),
              // headerShown: true,
              // headerTitle: "Số dư ví",
              // headerRight: () => (
              //   <Ionicons
              //     style={{ marginRight: 20, padding: 0 }}
              //     name={"wallet-outline"}
              //     size={24}

              //   />
              // )
            }}
          />
          <Tab.Screen
            name="CategoryTab"
            component={CategoriesScreen}
            options={{
              title: "Danh mục",
              tabBarIcon: ({ focused, color, size }) => (
                <Ionicons
                  name={focused ? "layers" : "layers-outline"}
                  color={color}
                  size={size}
                />
              ),
            }}
          />
        </>
      )}
    </Tab.Navigator>
  );
}

export default TabNavigator;
