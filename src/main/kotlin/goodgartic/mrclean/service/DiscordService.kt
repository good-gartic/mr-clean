package goodgartic.mrclean.service

import net.dv8tion.jda.api.JDA
import net.dv8tion.jda.api.entities.Activity
import net.dv8tion.jda.api.hooks.EventListener
import org.springframework.stereotype.Component
import java.util.logging.Logger

@Component
class DiscordService(private val jda: JDA, private val listeners: List<EventListener>) {

    private val logger: Logger = Logger.getLogger(this::class.qualifiedName)

    fun start() {
        logger.info("Starting the JDA instance and registering slash commands")

        registerEventListeners()
        updatePresence()
    }

    private fun registerEventListeners() {
        listeners.forEach {
            logger.info("Registering event listener [${it::class.qualifiedName}]")
            jda.addEventListener(it)
        }
    }

    private fun updatePresence() {
        jda.presence.activity = Activity.playing("Keeping the chats clean and shiny")
    }
}
