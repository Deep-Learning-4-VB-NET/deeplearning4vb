﻿Imports TimeProvider = org.nd4j.jita.allocator.time.TimeProvider

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

Namespace org.nd4j.jita.allocator.time.providers


	''' <summary>
	''' @author raver119@gmail.com
	''' </summary>
	Public Class OperativeProvider
		Implements TimeProvider

		Private time As New AtomicLong(0)


		''' <summary>
		''' This methods returns time in some, yet unknown, quants.
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property CurrentTime As Long Implements TimeProvider.getCurrentTime
			Get
				Return time.incrementAndGet()
			End Get
		End Property
	End Class

End Namespace