package goodgartic.mrclean.extensions

import goodgartic.mrclean.entities.Filter
import goodgartic.mrclean.entities.FilterSpecification

fun FilterSpecification.description(name: String, transform: (String) -> String): String =
    if (this.isEmpty()) "_Applies for all ${name}_"
    else {
        val allowed = this.allowedEntities.joinToString(", ") { transform(it) }
        val denied = this.deniedEntities.joinToString(", ") { transform(it) }

        """
        Applies for those $name: $allowed,
        Does not applies for those $name: $denied
        """.trimIndent()
    }

fun Filter.description(): String =
    """
    **ID**: `${this.id}`
    **Pattern**: `${this.pattern.replace("`", "\\`")}`
    **Delay**:    ${if (this.delay <= 0) "_Applied immediately_" else "Applied after **${this.delay} seconds**"}
    **Channels**: ${this.channels().description("channels") { channel -> "<#$channel>" }}
    **Users**:    ${this.users().description("users") { user -> "<@$user>" }}
    **Roles**:    ${this.roles().description("roles") { role -> "<@&$role>" }}
    """.trimIndent()
