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

Namespace org.nd4j.linalg.env

	Public Interface EnvironmentalAction
		''' <summary>
		''' This method returns target environemt variable for this action
		''' @return
		''' </summary>
		Function targetVariable() As String

		''' <summary>
		''' This method will be executed with corresponding Env Var value
		''' </summary>
		''' <param name="name"> </param>
		''' <param name="value"> </param>
		Sub process(ByVal value As String)
	End Interface

End Namespace