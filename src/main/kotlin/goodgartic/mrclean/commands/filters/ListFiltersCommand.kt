package goodgartic.mrclean.commands.filters

import goodgartic.mrclean.commands.SlashCommand
import goodgartic.mrclean.configuration.Constants
import goodgartic.mrclean.entities.Filter
import goodgartic.mrclean.entities.FilterSpecification
import goodgartic.mrclean.extensions.description
import goodgartic.mrclean.service.FilterService
import net.dv8tion.jda.api.EmbedBuilder
import net.dv8tion.jda.api.entities.MessageEmbed
import net.dv8tion.jda.api.entities.User
import net.dv8tion.jda.api.events.interaction.SlashCommandEvent
import net.dv8tion.jda.api.interactions.commands.build.CommandData
import org.springframework.stereotype.Component

@Component
class ListFiltersCommand(private val service: FilterService) : SlashCommand {

    override val definition: CommandData = CommandData("list-filters", "List all enabled message filters")

    override fun execute(event: SlashCommandEvent) {
        val interaction = event.deferReply().complete()

        val filters = service.allFilters()
        val embed = buildFiltersEmbed(filters, event.user)

        interaction.editOriginalEmbeds(embed).queue()
    }

    private fun buildFiltersEmbed(filters: List<Filter>, user: User): MessageEmbed {
        val single = filters.size == 1
        val title = "There ${if (single) "is" else "are"} ${filters.size} enabled message filter${if (single) "" else "s"}"
        val description = if (filters.isEmpty()) "You can enable new filters with the `/create-filter` slash command"
        else filtersDescription(filters)

        return EmbedBuilder()
            .setTitle(title)
            .setDescription(description)
            .setColor(Constants.Colors.primary)
            .setThumbnail(Constants.avatar)
            .setFooter("I was asked by ${user.asTag} btw")
            .build()
    }

    private fun filtersDescription(filters: List<Filter>): String {
        return filters.joinToString("\n\n") {
            it.description()
        }
    }
}
