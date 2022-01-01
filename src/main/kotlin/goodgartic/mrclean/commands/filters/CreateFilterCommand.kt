package goodgartic.mrclean.commands.filters

import goodgartic.mrclean.commands.SlashCommand
import goodgartic.mrclean.configuration.Constants
import goodgartic.mrclean.entities.Filter
import goodgartic.mrclean.service.FilterService
import net.dv8tion.jda.api.EmbedBuilder
import net.dv8tion.jda.api.entities.MessageEmbed
import net.dv8tion.jda.api.entities.User
import net.dv8tion.jda.api.events.interaction.SlashCommandEvent
import net.dv8tion.jda.api.interactions.commands.OptionType
import net.dv8tion.jda.api.interactions.commands.build.CommandData
import org.springframework.stereotype.Component
import java.time.Instant

@Component
class CreateFilterCommand(private val service: FilterService) : SlashCommand {

    override val definition: CommandData = CommandData("create-filter", "Creates a new message filter")
        .addOption(OptionType.STRING, "pattern", "Regex pattern used for matching against the message", true)
        .addOption(OptionType.INTEGER, "delay", "Delay in seconds, before the message gets deleted, defaults to 0, max is 2 minutes", false)
        .addOption(OptionType.CHANNEL, "repost-channel", "Channel, to which the message should be reposted", false)

    override fun execute(event: SlashCommandEvent) {
        val interaction = event.deferReply().complete()

        val pattern = event.getOption("pattern")?.asString ?: throw IllegalArgumentException("Missing the pattern parameter")
        val delay = event.getOption("delay")?.asLong?.coerceIn(0L..120L) ?: 0L
        val channel = event.getOption("repost-channel")?.asMessageChannel?.id ?: ""

        val filter = service.createFilter(pattern, delay, channel)

        interaction.editOriginalEmbeds(filterCreatedEmbed(filter, event.user)).queue()
    }

    private fun filterCreatedEmbed(filter: Filter, user: User): MessageEmbed {
        return EmbedBuilder()
            .setColor(Constants.Colors.green)
            .setThumbnail(Constants.avatar)
            .setTitle("New filter created")
            .setDescription("This filter was assigned an unique ID `${filter.id}`")
            .setFooter("Invoked by ${user.asTag}")
            .setTimestamp(Instant.now())
            .build()
    }
}