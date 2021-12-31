package goodgartic.mrclean.listeners

import goodgartic.mrclean.service.FilterService
import net.dv8tion.jda.api.JDA
import net.dv8tion.jda.api.entities.Guild
import net.dv8tion.jda.api.entities.Message
import net.dv8tion.jda.api.events.message.MessageReceivedEvent
import net.dv8tion.jda.api.events.message.MessageUpdateEvent
import net.dv8tion.jda.api.hooks.ListenerAdapter
import org.slf4j.LoggerFactory
import org.springframework.stereotype.Component

@Component
class MessagesListener(
    private val jda: JDA,
    private val guild: Guild,
    private val service: FilterService
) : ListenerAdapter() {

    private val logger = LoggerFactory.getLogger(this::class.java)

    // When a message is created (sent)
    override fun onMessageReceived(event: MessageReceivedEvent) {
        processMessage(event.message)
    }

    // When a message is edited (sent)
    override fun onMessageUpdate(event: MessageUpdateEvent) {
        processMessage(event.message)
    }

    private fun processMessage(message: Message) {
        // Do not handle messages that are either:
        // - from Mr. Clean itself (this could cause infinite recursion)
        // - from another guild
        // - from DMs
        if (jda.selfUser.id == message.author.id || !message.isFromGuild || message.guild.id != guild.id) {
            return
        }

        val member = message.member ?: return logger.warn("Cannot obtain member for message [${message.id}]")
        val filter = service.matchFilter(message.contentDisplay, message.channel.id, member.id, member.roles.map { it.id })

        if (filter != null) {
            logger.debug("Processing message [id = ${message.id}] because it matched filter [id = ${filter.id}]")
        }
    }
}