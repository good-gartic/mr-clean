package goodgartic.mrclean.commands.filters

import goodgartic.mrclean.commands.SlashCommand
import goodgartic.mrclean.configuration.Constants
import goodgartic.mrclean.entities.Filter
import goodgartic.mrclean.extensions.description
import goodgartic.mrclean.service.FilterService
import net.dv8tion.jda.api.EmbedBuilder
import net.dv8tion.jda.api.entities.MessageEmbed
import net.dv8tion.jda.api.events.interaction.SlashCommandEvent
import net.dv8tion.jda.api.interactions.commands.OptionType
import net.dv8tion.jda.api.interactions.commands.build.CommandData
import org.springframework.stereotype.Component
import java.time.Instant

@Component
class EditFilterCommand(private val service: FilterService) : SlashCommand {

    override val definition: CommandData = CommandData("edit-filter", "Edits a previously created message filter")
        .addOption(OptionType.INTEGER, "id", "ID of the filter that should be edited", true)
        .addOption(OptionType.STRING, "pattern", "Regex pattern that should be matched", false)
        .addOption(OptionType.INTEGER, "delay", "Delay before deleting/reposting the message", false)
        .addOption(OptionType.CHANNEL, "repost-channel", "Channel, to which the messages should be reposted, leave empty to disable reposting", false)

    override fun execute(event: SlashCommandEvent) {
        val filter = event.getOption("id")?.asLong?.let { service.findFilter(it) }
            ?: throw IllegalArgumentException("Cannot find the specified filter")

        val pattern = event.getOption("pattern")?.asString ?: filter.pattern
        val delay = event.getOption("delay")?.asLong ?: filter.delay
        val channel = event.getOption("repost-channel")?.asMessageChannel?.id ?: ""

        val updated = service.updateFilter(filter.copy(pattern = pattern, delay = delay, repostChannel = channel))

        event.replyEmbeds(filterUpdatedEmbed(updated)).queue()
    }

    private fun filterUpdatedEmbed(filter: Filter): MessageEmbed =
        EmbedBuilder()
            .setTitle("Filter updated")
            .setColor(Constants.Colors.green)
            .setDescription(filter.description())
            .setTimestamp(Instant.now())
            .build()
}