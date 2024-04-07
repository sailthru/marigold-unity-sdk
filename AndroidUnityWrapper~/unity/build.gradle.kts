plugins {
    id("com.android.library")
    id("org.jetbrains.kotlin.android")
}

android {
    compileSdk = 34
    buildToolsVersion = "34.0.0"
    namespace = "com.marigold.sdk.unity"

    defaultConfig {
        minSdk = 21
    }
    buildTypes {
        getByName("release") {
            isMinifyEnabled = false
            proguardFiles(getDefaultProguardFile("proguard-android.txt"), "proguard-rules.pro")
        }
    }
    compileOptions {
        sourceCompatibility = JavaVersion.VERSION_17
        targetCompatibility = JavaVersion.VERSION_17
    }
    testOptions {
        unitTests.apply {
            isReturnDefaultValues = true
        }
    }
}

tasks.register<Delete>("clearJar") {
    delete("../../Plugins/Android/libs/MarigoldWrapper.jar")
}

tasks.register<Copy>("makeJar") {
    dependsOn("clearJar", "build")

    from("build/intermediates/aar_main_jar/release/")
    into("../../Plugins/Android/libs/")
    include("classes.jar")
    rename ("classes.jar", "MarigoldWrapper.jar")
}

repositories {
    maven {
        url = uri("https://github.com/carnivalmobile/maven-repository/raw/master/")
    }
}

dependencies {
    implementation(fileTree(mapOf("dir" to "libs", "include" to listOf("*.jar"))))
    implementation("com.marigold.sdk:marigold:20.2.0")
    implementation("androidx.core:core-ktx:1.12.0")

    testImplementation("junit:junit:4.13.2")
    testImplementation("org.mockito:mockito-inline:4.1.0")
    testImplementation("org.mockito.kotlin:mockito-kotlin:4.0.0")
    testImplementation("org.robolectric:robolectric:4.10.3")
    testImplementation("androidx.test:core:1.5.0")
}