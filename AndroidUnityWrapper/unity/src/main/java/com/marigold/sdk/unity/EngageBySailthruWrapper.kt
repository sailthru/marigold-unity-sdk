package com.marigold.sdk.unity

import android.net.Uri
import com.marigold.sdk.EngageBySailthru
import com.marigold.sdk.EngageBySailthru.TrackHandler
import com.marigold.sdk.Marigold
import com.marigold.sdk.model.Purchase
import com.unity3d.player.UnityPlayer
import org.json.JSONArray
import org.json.JSONException
import org.json.JSONObject
import java.lang.IllegalArgumentException
import java.lang.reflect.Constructor
import java.lang.reflect.InvocationTargetException
import java.net.URI
import java.net.URISyntaxException

@Suppress("unused")
object EngageBySailthruWrapper {
    private const val ENGAGE_ST_UNITY = "EngageBySailthru"
    private const val ENGAGE_ST_RECEIVE_VARS = "ReceiveProfileVars"
    private const val ENGAGE_ST_RECEIVE_LINK = "ReceiveUnwrappedLink"

    fun trackPageview(url: String, tags: String?) {
        val uri =  try {
            URI(url)
        } catch (e: Exception) {
            MarigoldWrapper.sendErrorMessage(e)
            return
        }

        val tagsList = getStringListFromJSONArrayString(tags)

        engageBySailthru()?.trackPageview(uri, tagsList, object : TrackHandler {
            override fun onSuccess() {}
            override fun onFailure(error: Error) {
                MarigoldWrapper.sendErrorMessage(error)
            }
        })
    }

    fun trackImpression(sectionId: String, urls: String?) {
        val uris = getStringListFromJSONArrayString(urls)?.mapNotNull { url ->
            try {
                 URI(url)
            } catch (e: URISyntaxException) {
                MarigoldWrapper.sendErrorMessage(e)
                null
            }
        }


        engageBySailthru()?.trackImpression(sectionId, uris, object : TrackHandler {
            override fun onSuccess() {}
            override fun onFailure(error: Error) {
                MarigoldWrapper.sendErrorMessage(error)
            }
        })
    }

    fun trackClick(sectionId: String, url: String) {
        val uri =  try {
            URI(url)
        } catch (e: Exception) {
            MarigoldWrapper.sendErrorMessage(e)
            return
        }

        engageBySailthru()?.trackClick(sectionId, uri, object : TrackHandler {
            override fun onSuccess() {}
            override fun onFailure(error: Error) {
                MarigoldWrapper.sendErrorMessage(error)
            }
        })
    }

    fun setUserEmail(userEmail: String?) {
        engageBySailthru()?.setUserEmail(userEmail, object : Marigold.MarigoldHandler<Void?> {
            override fun onSuccess(value: Void?) {}
            override fun onFailure(error: Error) {
                MarigoldWrapper.sendErrorMessage(error)
            }
        })
    }

    fun setUserId(userId: String?) {
        engageBySailthru()?.setUserId(userId, object : Marigold.MarigoldHandler<Void?> {
            override fun onSuccess(value: Void?) {}
            override fun onFailure(error: Error) {
                MarigoldWrapper.sendErrorMessage(error)
            }
        })
    }

    fun logEvent(value: String) {
        engageBySailthru()?.logEvent(value)
    }

    fun logEvent(value: String, varsString: String) {
        val vars = getVarsJson(varsString) ?: return
        engageBySailthru()?.logEvent(value, vars)
    }

    fun setProfileVars(varsString: String) {
        val vars = getVarsJson(varsString) ?: return
        engageBySailthru()?.setProfileVars(vars, object : Marigold.MarigoldHandler<Void?> {
            override fun onSuccess(value: Void?) {}
            override fun onFailure(error: Error) {
                MarigoldWrapper.sendErrorMessage(error)
            }
        })
    }

    fun getProfileVars() {
        engageBySailthru()?.getProfileVars(object : Marigold.MarigoldHandler<JSONObject?> {
            override fun onSuccess(value: JSONObject?) {
                value?.let { vars ->
                    UnityPlayer.UnitySendMessage(ENGAGE_ST_UNITY, ENGAGE_ST_RECEIVE_VARS, vars.toString())
                }
            }
            override fun onFailure(error: Error) {
                MarigoldWrapper.sendErrorMessage(error)
            }
        })
    }

    fun handleSailthruLink(linkString: String) {
        val uri = Uri.parse(linkString)
        val unwrappedLink = try {
            val engageBySailthru = engageBySailthru() ?: return
            engageBySailthru.handleSailthruLink(uri, object : Marigold.MarigoldHandler<Void?> {
                override fun onSuccess(value: Void?) {}
                override fun onFailure(error: Error) {
                    MarigoldWrapper.sendErrorMessage(error)
                }
            })
        } catch (e: IllegalArgumentException) {
            MarigoldWrapper.sendErrorMessage(e)
            return
        }
        UnityPlayer.UnitySendMessage(ENGAGE_ST_UNITY, ENGAGE_ST_RECEIVE_LINK, unwrappedLink.toString())
    }

    fun logPurchase(purchaseString: String) {
        val purchase = getPurchase(purchaseString) ?: return
        engageBySailthru()?.logPurchase(purchase, object : Marigold.MarigoldHandler<Void?> {
            override fun onSuccess(value: Void?) {}
            override fun onFailure(error: Error) {
                MarigoldWrapper.sendErrorMessage(error)
            }
        })
    }

    fun logAbandonedCart(purchaseString: String) {
        val purchase = getPurchase(purchaseString) ?: return
        engageBySailthru()?.logAbandonedCart(purchase, object : Marigold.MarigoldHandler<Void?> {
            override fun onSuccess(value: Void?) {}
            override fun onFailure(error: Error) {
                MarigoldWrapper.sendErrorMessage(error)
            }
        })
    }

    private fun getVarsJson(varsString: String): JSONObject? = try {
        JSONObject(varsString)
    } catch (e: JSONException) {
        MarigoldWrapper.sendErrorMessage(e)
        null
    }

    private fun getPurchase(purchaseString: String): Purchase? = try {
        val purchaseJson = JSONObject(purchaseString)
        val constructor: Constructor<Purchase> =
            Purchase::class.java.getDeclaredConstructor(JSONObject::class.java)
        constructor.isAccessible = true
        constructor.newInstance(purchaseJson)
    } catch (e: InvocationTargetException) {
        MarigoldWrapper.sendErrorMessage(e.cause ?: Error("Error creating purchase instance"))
        null
    } catch (e: Exception) {
        MarigoldWrapper.sendErrorMessage(e)
        null
    }

    private fun getStringListFromJSONArrayString(arrayString: String?): List<String>? {
        arrayString ?: return null

        return try {
            val list = mutableListOf<String>()
            val jsonArray = JSONArray(arrayString)
            for (i in 0..jsonArray.length()) {
                list.add(jsonArray.getString(i))
            }
            list
        } catch (e: Exception) {
            null
        }
    }

    private fun engageBySailthru(): EngageBySailthru? = try {
        EngageBySailthru()
    } catch (e: Exception) {
        MarigoldWrapper.sendErrorMessage(e)
        null
    }
}