# zfsclean
Small command line tool that helps cleaning up a large collection of snapshots


# notes

https://git.bashclub.org/bashclub/zfs-housekeeping

```bash
zfs list -t snapshot -o name,creation
zfs get written

```


zfsclean --keep=30d --filter="rpool/data/" --format-string="zfs destroy {name}" > cleanup.sh