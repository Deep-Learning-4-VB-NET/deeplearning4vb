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

Imports System.Collections.Generic

Namespace org.datavec.api.io.labels

    ''' <summary>Derives several labels from the path (or URI) of a file.</summary>
    Public Interface PathMultiLabelGenerator

        ''' <param name="uriPath"> The file or URI path to get the label for </param>
        ''' <returns> A list of labels for the specified URI/path </returns>
        Function getLabels(ByVal uriPath As String) As IList(Of writable.Writable)

    End Interface

End Namespace
