import React, { useContext } from "react";
import { View, ActivityIndicator } from "react-native";
import { NavigationContainer } from "@react-navigation/native";

import AppStackNav from "./AppStackNav";
import { AuthContext } from "../context/AuthContext";

function AppNav() {
  const { isLoading } = useContext(AuthContext);

  if (isLoading) {
    return (
      <View style={{ flex: 1, justifyContent: "center", alignItems: "center" }}>
        <ActivityIndicator size={"large"} />
      </View>
    );
  }

  return (
    <NavigationContainer>
      <AppStackNav />
    </NavigationContainer>
  );
}

export default AppNav;
