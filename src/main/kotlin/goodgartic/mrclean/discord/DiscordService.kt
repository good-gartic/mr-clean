package goodgartic.mrclean.discord

import net.dv8tion.jda.api.JDA
import net.dv8tion.jda.api.entities.Activity
import org.springframework.stereotype.Component
import java.util.logging.Logger

@Component
class DiscordService(private val jda: JDA) {

    private val logger: Logger = Logger.getLogger(this::class.qualifiedName)

    fun start() {
        logger.info("Starting the JDA instance and registering slash commands")
        jda.presence.activity = Activity.playing("Keeping the chats clean and shiny")
    }
}