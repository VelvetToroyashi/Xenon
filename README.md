# Xenon - The VC utilities bot

Xenon is a Discord bot designed to assist with moderation actions specifically tailored to Voice Channels on Discord. 

Voice channels suck, and so does Discord's tooling regarding voice channels (why can't we mute server un/mute someone when they're not in VC?)

Xenon helps bridge some gaps with regards to moderating voice channels including, but not limited to:

- Timed VC mutes, regardless of the user's current voice state
- Anti-sidestepping of server mutes (aka, sticky mutes, because for some reason leaving and rejoining a server clears this??)
- Automatically set channels to be push to talk when they exceed a user threshold (moderators are unaffected by this!)
- Creating "private"/ephemeral 

## Checklist

- [ ] Allow VC Mods to give timed mutes which automatically expire
- [ ] Allow the retroactive lookup of prior infractions for a given user
- [ ] Potentially(?) log these infractions instead of having an audit log
- [ ] Allow unmetered channels to become push to talk when a certain threshold is met (e.g., 12 people, PTT time)

## Setup

To set up Xenon for your Discord server, follow these steps:

1. **Deploy the Docker Container**: Xenon is available as a Docker container. You can use the following Docker image: `velvettoroyashi/xenon`.

2. **Environment Variables**: Ensure the following environment variables are set with appropriate values:

   - `XENON_MuteRoleID`: ID of the role assigned to muted users.
   - `XENON_JanitorRoleID`: ID of the role assigned to janitors or moderators.
   - `XENON_LoggingChannelID`: ID of the channel where Xenon logs moderation actions.
   - `XENON_DiscordToken`: Your Discord Bot Token.

> ![NOTE]
> These values are bound to change (and, in fact, be removed in favor of a more robust, per-server configuration)

3. **Invite Xenon to Your Server**: Generate an invite link for the bot on the [Developer Dashboard](https://discord.dev), and invite it.

4. **You're off to the races**: Xenon should just work:tm:, so all you have to do is use the commands. 


## Feedback and Support

If you encounter any issues, have suggestions for improvements, or need assistance with Xenon, feel free to [open an issue](https://github.com/VelvetToroyashi/Xenon/issues/new) or [join my projects Discord server](https://discord.gg/MMw2aXuHSQ). 

## Contributions

Contributions to Xenon are welcome! If you'd like to contribute code, report bugs, or suggest new features, please open an Issue or PR as appropriate.

---

Xenon is developed and maintained with support from JetBrains software and donations, if you'd like to help me continue to make projects, consider donating through [GitHub](https://github.com/sponsors/VelvetToroyashi) or [Ko-Fi](https://ko-fi.com/VelvetToroyashi)!
