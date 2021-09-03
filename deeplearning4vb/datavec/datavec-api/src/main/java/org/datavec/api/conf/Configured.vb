﻿'
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

	Public Class Configured
		Implements Configurable

'JAVA TO VB CONVERTER NOTE: The field conf was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private conf_Conflict As Configuration

		''' <summary>
		''' Construct a Configured. </summary>
		Public Sub New()
			Me.New(Nothing)
		End Sub

		''' <summary>
		''' Construct a Configured. </summary>
		Public Sub New(ByVal conf As Configuration)
			Me.Conf = conf
		End Sub

		' inherit javadoc
		Public Overridable Property Conf Implements Configurable.setConf As Configuration
			Set(ByVal conf As Configuration)
				Me.conf_Conflict = conf
			End Set
			Get
				Return conf_Conflict
			End Get
		End Property


	End Class

End Namespace