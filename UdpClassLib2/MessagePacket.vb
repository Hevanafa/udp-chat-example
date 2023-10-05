Public Class MessagePacket
    Private _display_name$  ' 30
    Private _contents$      ' 255

    Public Property display_name$
        Get
            Return _display_name
        End Get

        Set(value$)
            _display_name = Left(Trim(value$), 30)
        End Set
    End Property

    Public Property contents$
        Get
            Return _contents
        End Get

        Set(value$)
            _contents = Left(Trim(value$), 255)
        End Set
    End Property



End Class
