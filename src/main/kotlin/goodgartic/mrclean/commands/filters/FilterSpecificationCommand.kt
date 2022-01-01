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
class FilterSpecificationCommand(private val service: FilterService) : SlashCommand {

    override val definition: CommandData = CommandData("specify-filter", "Allows for a fine-grained filter configuration, see the options for more info")
        .addOption(OptionType.INTEGER, "id", "ID of the filter to which the changes should be applied to", true)
        .addOption(OptionType.BOOLEAN, "reset", "Reset all specifications for this filter (except those specified in this command invocation)", false)
        .addOption(OptionType.USER, "include-user", "Add user to the filter whitelist", false)
        .addOption(OptionType.USER, "exclude-user", "Add user to the filter blacklist", false)
        .addOption(OptionType.ROLE, "include-role", "Add role to the filter whitelist", false)
        .addOption(OptionType.ROLE, "exclude-role", "Add role to the filter blacklist", false)
        .addOption(OptionType.CHANNEL, "include-channel", "Add channel to the filter whitelist", false)
        .addOption(OptionType.CHANNEL, "exclude-channel", "Add channel to the filter blacklist", false)


    @Suppress("NAME_SHADOWING")
    override fun execute(event: SlashCommandEvent) {
        val interaction = event.deferReply().complete()

        val filter = event.getOption("id")?.asLong?.let { service.findFilter(it) }
            ?: throw IllegalArgumentException("Cannot find the specified filter")

        // The '== true' triggers nullability check here
        // If the reset parameter was provided and set to true, remove all previously declared specifications
        val initial =
            if (event.getOption("reset")?.asBoolean == true) filter.copy(channels = "" , users = "", roles = "")
            else filter

        // TODO: Refactor this to a little bit more readable version maybe?

        // Process all new inclusions/exclusions
        val changes = listOf<Pair<String?, (String, Filter) -> Filter>>(
            event.getOption("include-user")?.asUser?.id to { user, filter -> filter.addIncludedUser(user) },
            event.getOption("exclude-user")?.asUser?.id to { user, filter -> filter.addExcludedUser(user) },
            event.getOption("include-role")?.asRole?.id to { role, filter -> filter.addIncludedRole(role) },
            event.getOption("exclude-role")?.asRole?.id to { role, filter -> filter.addExcludedRole(role) },
            event.getOption("include-channel")?.asMessageChannel?.id to { channel, filter -> filter.addIncludedChannel(channel) },
            event.getOption("exclude-channel")?.asMessageChannel?.id to { channel, filter -> filter.addExcludedChannel(channel) },
        )

        // Apply all changes one by one passing the composed filter rules
        val updated = service.updateFilter(changes.fold(initial) {
            filter, (id, lambda) -> id?.let { lambda(it, filter) } ?: filter
        })

        interaction.editOriginalEmbeds(filterUpdated(updated)).complete()
    }

    private fun filterUpdated(filter: Filter): MessageEmbed =
        EmbedBuilder()
            .setColor(Constants.Colors.green)
            .setTitle("Filter `${filter.id}` updated")
            .setDescription(filter.description())
            .setThumbnail(Constants.avatar)
            .setTimestamp(Instant.now())
            .build()
}