'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Namespace org.datavec.api.conf

    ''' <summary>Base class for things that may be configured with a <see cref="Configuration"/>.</summary>
    Public Class Configured
        Implements Configurable

        Private confValue As Configuration

        ''' <summary>Construct a Configured.</summary>
        Public Sub New()
            Me.New(Nothing)
        End Sub

        ''' <summary>Construct a Configured.</summary>
        Public Sub New(ByVal conf As Configuration)
            setConf(conf)
        End Sub

        Public Overridable Sub setConf(ByVal conf As Configuration) Implements Configurable.setConf
            Me.confValue = conf
        End Sub

        Public Overridable Function getConf() As Configuration Implements Configurable.getConf
            Return confValue
        End Function

        Public Overridable Property Conf As Configuration Implements Configurable.Conf
            Get
                Return getConf()
            End Get
            Set(ByVal value As Configuration)
                setConf(value)
            End Set
        End Property

    End Class

End Namespace
