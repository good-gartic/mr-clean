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
class DeleteFilterCommand(private val service: FilterService) : SlashCommand {

    override val definition: CommandData = CommandData("delete-filter", "Delete the specified filter")
        .addOption(OptionType.INTEGER, "id", "ID of the filter that should be deleted", true)

    override fun execute(event: SlashCommandEvent) {
        val interaction = event.deferReply().complete()

        val filter = event.getOption("id")?.asLong?.let { service.findFilter(it) }
            ?: throw IllegalArgumentException("Cannot find the specified filter")

        service.deleteFilter(filter)

        interaction.editOriginalEmbeds(filterDeletedEmbed(filter)).queue()
    }

    private fun filterDeletedEmbed(filter: Filter): MessageEmbed =
        EmbedBuilder()
            .setColor(Constants.Colors.red)
            .setTitle("Filter `${filter.id}` deleted")
            .setDescription(filter.description())
            .setThumbnail(Constants.avatar)
            .setTimestamp(Instant.now())
            .build()
}