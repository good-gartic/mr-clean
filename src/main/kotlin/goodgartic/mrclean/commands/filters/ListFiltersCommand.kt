package goodgartic.mrclean.commands.filters

import goodgartic.mrclean.commands.SlashCommand
import goodgartic.mrclean.configuration.Constants
import goodgartic.mrclean.entities.Filter
import goodgartic.mrclean.entities.FilterSpecification
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

        fun FilterSpecification.description(name: String, transform: (String) -> String): String =
            if (this.isEmpty()) "Applies for all $name"
            else {
                val allowed = this.allowedEntities.joinToString(", ") { transform(it) }
                val denied = this.deniedEntities.joinToString(", ") { transform(it) }

                """
                Applies for those $name: $allowed,
                Does not applies for those $name: $denied
                """.trimIndent()
            }

        return filters.joinToString("\n\n") {
            """
            Pattern: `${it.pattern.replace("`", "\\`")}`
            Delay:    ${if (it.delay <= 0) "Applied immediately" else "${it.delay} seconds"}
            Channels: ${it.channels().description("channels") { channel -> "<#$channel>" }}
            Users:    ${it.users().description("users") { user -> "<@$user>" }}
            Roles:    ${it.roles().description("roles") { role -> "<@&$role>" }}
            """.trimIndent()
        }
    }
}
