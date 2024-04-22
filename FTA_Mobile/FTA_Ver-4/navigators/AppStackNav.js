import React from "react";
import { createNativeStackNavigator } from "@react-navigation/native-stack";
import { IconButton } from "react-native-paper";
import { Ionicons } from "@expo/vector-icons";

import TabNavigator from "./TabNavigator";
import AuthStack from "./AuthStack";
import ProductDetailScreen from "../screens/ProductDetailScreen";
import CatListProdScreen from "../screens/CatListProdScreen";
import CategoriesScreen from "../screens/CategoriesScreen";
import NotificationScreen from "../screens/NotificationScreen";
import CartScreen from "../screens/CartScreen";
import OrderScreen from "../screens/OrderScreen";
import AddressScreen from "../screens/AddressScreen";
import SearchScreen from "../screens/SearchScreen";
import ReceiveInfoScreen from "../screens/ReceiveInfoScreen";
import UserInfoScreen from "../screens/UserInfoScreen";
import { DefaultTheme } from "../themes/DefaultTheme";
import HistoryOrderScreen from "../screens/HistoryOrderScreen";

const Stack = createNativeStackNavigator();

function AppStackNav() {
  return (
    <Stack.Navigator
      screenOptions={({ navigation }) => ({
        headerBackTitleVisible: false,
        // headerStyle: {
        //   backgroundColor: DefaultTheme.headerBgColor,
        //   height: 50,
        // },
        // headerLeft: () => (
        //   <Ionicons
        //     style={{ margin: 0, padding: 0 }}
        //     name={"chevron-back-outline"}
        //     size={28}
        //     onPress={() => {
        //       navigation.goBack();
        //     }}
        //   />
        // ),
        headerTintColor: "#000",
        headerTitleStyle: {
          fontSize: 18,
          fontWeight: "bold",
        },
      })}
    >
      <Stack.Screen
        name="Home"
        component={TabNavigator}
        options={{
          headerShown: false,
        }}
      />
      <Stack.Screen name="AuthScreen" component={AuthStack} />
      <Stack.Screen
        name="Notification"
        component={NotificationScreen}
        options={{ title: "Thông báo" }}
      />
      <Stack.Screen name="ProductDetail" component={ProductDetailScreen} />
      <Stack.Screen name="CatListProdScreen" component={CatListProdScreen} />
      <Stack.Screen name="CategoriesScreen" component={CategoriesScreen} />
      <Stack.Screen
        name="CartScreen"
        component={CartScreen}
        options={{
          title: "Giỏ hàng",
        }}
      />
      <Stack.Screen
        name="OrderScreen"
        component={OrderScreen}
        options={{
          title: "Thông tin đơn hàng",
          headerLeft: () => null,
        }}
      />
      <Stack.Screen
        name="SearchScreen"
        component={SearchScreen}
        options={{
          title: "Tìm kiếm sản phẩm",
        }}
      />
      <Stack.Screen
        name="AddressScreen"
        component={AddressScreen}
        options={{ title: "Địa chỉ" }}
      />
      <Stack.Screen
        name="UserInfoScreen"
        component={UserInfoScreen}
        options={{
          title: "Thông tin cá nhân",
        }}
      />
      <Stack.Screen
        name="ReceiveInfoScreen"
        component={ReceiveInfoScreen}
        options={{
          title: "Thanh toán",
        }}
      />
      <Stack.Screen
        name="HistoryOrderScreen"
        component={HistoryOrderScreen}
        options={{
          title: "Lịch sử mua hàng",
        }}
      />
    </Stack.Navigator>
  );
}

export default AppStackNav;
